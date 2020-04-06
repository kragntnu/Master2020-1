using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace Master.Components
{
    public class MeshSweep2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshSweep class.
        /// </summary>
        public MeshSweep2()
          : base("MeshSweep2", "MSweep",
              "Take in 4 source points, 4 target points and create mesh",
              "NTNU", "Master")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PointsSource", "spts", "4 points", GH_ParamAccess.list);//0
            pManager.AddPointParameter("PointsTarget", "tpts", "4 points", GH_ParamAccess.list);//1
            pManager.AddIntegerParameter("U", "u", "Cross sectional division horizontal", GH_ParamAccess.item);//2
            pManager.AddIntegerParameter("V", "v", "Cross sectional division vertical", GH_ParamAccess.item);//3
            pManager.AddIntegerParameter("Divisions", "w", "How many divisions of the mesh along sweep longitudinal", GH_ParamAccess.item);//4

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("Element", "el", "Element class", GH_ParamAccess.item);
            pManager.AddGenericParameter("AllNodesOneList", "nl", "nodelist", GH_ParamAccess.list); //0
            pManager.AddGenericParameter("NodeTree", "nt", "nodelist", GH_ParamAccess.tree); //1
            pManager.AddGenericParameter("BrepTree", "bt", "nodelist", GH_ParamAccess.tree); //2
            pManager.AddGenericParameter("LoftedSurfaces", "srf", "srf", GH_ParamAccess.list); //3
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //declare variables
            List<Point3d> spts = new List<Point3d>();
            List<Point3d> tpts = new List<Point3d>();
            int u = 1; //minimum value cross section
            int v = 1; //minimum value cross section
            int w = 1; //minimum value longitudinal

            // get inputs
            DA.GetDataList(0, spts); //not ref when points
            DA.GetDataList(1, tpts); //not ref when points
            DA.GetData(2, ref u);
            DA.GetData(3, ref v);
            DA.GetData(4, ref w);

            DataTree<Curve> edgecrvtree = new DataTree<Curve>();
            DataTree<Point3d> vrttree = new DataTree<Point3d>();
            DataTree<Point3d> vrtsectiontree = new DataTree<Point3d>(); //all vertices sorted in branches of correct surface
            List<NurbsSurface> s = new List<NurbsSurface>(); //list of all surfaces along length

            //make edgecurves, get their length and make new corner points

            for (int i = 0; i < 4; i++) //4 corner points
            {
                edgecrvtree.Add(new Line(spts[i], tpts[i]).ToNurbsCurve(), new GH_Path(i));
                vrttree.AddRange(edgecrvtree.Branch(i)[0].DivideEquidistant(edgecrvtree.Branch(i)[0].GetLength() / w), new GH_Path(i));
                int listitem = 0;
                for (int j = 0; j < (w + 1); j++)
                {
                    vrtsectiontree.Add(vrttree.Branch(i)[listitem], new GH_Path(j)); //all vertices sorted in branches of correct surface
                    listitem++;
                }
            }

            //make surfaces in all divisions
            for (int j = 0; j < (w + 1); j++)
            {
                s.Add(NurbsSurface.CreateFromCorners(vrtsectiontree.Branch(j)[0], vrtsectiontree.Branch(j)[1], vrtsectiontree.Branch(j)[2], vrtsectiontree.Branch(j)[3], 0));
            } // (branch number) [list number]

            DataTree<Brep> btree = new DataTree<Brep>();
            DataTree<Curve> loftcrv = new DataTree<Curve>(); //loft to get solid
            List<Point3d> nodenr = new List<Point3d>();
            DataTree<Point3d> nodetree = new DataTree<Point3d>();


            for (int a = 0; a < (w + 1); a++)
            {
                for (int i = 0; i < (v + 1); i++)
                {
                    for (int j = 0; j < (u + 1); j++)
                    {
                        Point3d p = new Point3d(s[a].PointAt(s[a].Domain(0).Length / v * i, s[a].Domain(1).Length / u * j)); // v-value first, then u value, because domain(0) is v
                        nodenr.Add(p); //all nodes is one list (all nodes correctly listed: in u-direction, v-direction, then w-direction). Here: should use node class? 
                        nodetree.Add(p, new GH_Path(a, i)); //nodes in branches, first branch is in w-direction, then second branch is v-direction, to easier sort the elements
                    }
                }
            }

            for (int a = 0; a < (w + 1); a++)
            {
                for (int i = 0; i < v; i++)
                {
                    for (int j = 0; j < u; j++)
                    {
                        Brep b = Brep.CreateFromCornerPoints(nodetree.Branch(a, i)[j], nodetree.Branch(a, i + 1)[j], nodetree.Branch(a, i + 1)[j + 1], nodetree.Branch(a, i)[j + 1], 0);
                        //used to create breps using correct vertices,  use same code to get correct elements sorted.
                        btree.Add(b, new GH_Path(a, i));
                        loftcrv.AddRange(btree.Branch(a, i)[j].DuplicateEdgeCurves(), new GH_Path(a));
                    }
                }
            }

            List<Brep> srf = new List<Brep>();
            for (int i = 0; i < loftcrv.Branch(0).Count; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    srf.AddRange(Brep.CreateFromLoft(new List<Curve> { loftcrv.Branch(0 + j)[i], loftcrv.Branch(1 + j)[i] }, Point3d.Unset, Point3d.Unset, LoftType.Normal, false));
                }
            } // Surfaces in the middle (not outer edges are doubled

            DA.SetDataList(0, nodenr);
            DA.SetDataTree(1, nodetree);
            DA.SetDataTree(2, btree);
            DA.SetDataList(3, srf);
        }
        /*
            Element E1 = new Element();
            E1.id = 1;
            Node n1 = new Node(0,new Point3d(1,1,1));
            Element E2 = new Element(2);
            List<Point3d> list1 = new List<Point3d>();
            DA.GetDataList(0, list1);
            E1.Node1 = list1[0];
            E1.Node2 = list1[1];
            E1.Node3 = list1[2];
            E1.Node4 = list1[3];
            E1.Node5 = list1[4];
            E1.Node6 = list1[5];
            E1.Node7 = list1[6];
            E1.Node8 = list1[7];
            DA.SetData(0, E1);
            */


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EA88640E-5B77-45BB-8066-B01041EE0503"); }
        }
    }
}