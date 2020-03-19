using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System.Linq;
using System.Threading.Tasks; 

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Master
{
    public class MeshPoints : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MeshPoints()
          : base("MeshPoints", "MeshP","Mesh from points","NTNU", "Master")
        {
        }
        //make a mesh from 4 corner points, to a divided surface with correct vertices. Can from this make 1 source mesh, 1 target mesh, and then use MeshSweep to sweep between the meshes?

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "Points which will create Mesh", GH_ParamAccess.list); //0
            //has to be listed in correct order
            //pManager.AddNumberParameter("Diameter", "d", "diameter input", GH_ParamAccess.item); //kan evnt stå list istedenfor item
            pManager.AddPointParameter("U count", "u", "divide curve in u direction", GH_ParamAccess.item); //1
            pManager.AddPointParameter("V count", "v", "divide curve in v direction", GH_ParamAccess.item); //2
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh", "mesh", "Mesh with correct vertices", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //declare variables
            List<Point3d> points = new List<Point3d>();
            int u;
            int v;
            // examples double fx = 11.92; //Mpa; //var lnum = new List<double>();
            // double fx = 11.92; //MPa
            // get inputs
            DA.GetData(0, ref points); //use reference not new
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);

           
        

            Brep b1 = Brep.CreateFromCornerPoints(points[0], points[1], points[2], points[3]); //Doesn't seem like it finds the CreateFromCornerPoints command..
            //then divb1= divide domain(b1,u,v) Using grasshopper component divide domain^2.  how to do this?
            //IniMesh = SubSrf(b1,divb1) Using grasshopper component isotrim. How to do this?
            // Copy from Roksvaag code:  private void RunScript(Brep b1, List<Brep> IniMesh, double Offset1, double Offset2, ref object Mesh)
            
            double ycrd, zcrd, rad;
            Brep Obj;

            //Create output tree
            DataTree<System.Object> tree = new DataTree<System.Object>();

            //Create Lists
            List<Point3d> Vrt = new List<Point3d>();
            List<Point3d> Pts = new List<Point3d>();
            List<double> Ang = new List<double>();


       

            //Sort vertices according to the yz-coordinate system w/points in the 3rd quadrant being first pt
            Point3d Cntr = AreaMassProperties.Compute(Section).Centroid;
            tree.Add(Section, new GH_Path(0));
            for (int i = 0; i < IniMesh.Count(); i++)
            {
                Cntr = AreaMassProperties.Compute(IniMesh[i]).Centroid;
                for (int j = 0; j < IniMesh[i].DuplicateVertices().Count(); j++)
                {
                    Vrt.Add(IniMesh[i].DuplicateVertices()[j]);
                    ycrd = Vrt[j].Y - Cntr.Y;
                    zcrd = Vrt[j].Z - Cntr.Z;
                    rad = Math.Atan2(zcrd, ycrd);
                    Ang.Add(rad);
                }
                var Sorted = Vrt.Zip(Ang, (x, y) => new { x, y })
                  .OrderBy(pair => pair.y)
                  .Reverse()
                  .ToList();
                Pts = Sorted.Select(pair => pair.x).ToList();
                Obj = Brep.CreateFromCornerPoints(Pts[0], Pts[1], Pts[2], Pts[3], doc.ModelAbsoluteTolerance);
                tree.Add(Obj, new GH_Path(1));
                Pts.Clear();
                Ang.Clear();
                Vrt.Clear();
           
            }

            Mesh = tree;

       


        //set outputs
        DA.SetData(0, Mesh);

            
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("B81EF9B6-E993-4978-83D8-7FF97636A768"); }
        }
    }
}

