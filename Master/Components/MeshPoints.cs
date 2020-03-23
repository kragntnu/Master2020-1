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
          : base("MeshPoints", "MeshP", "Mesh from points", "NTNU", "Master")
        {
        }
        //make a mesh from 4 corner points. Can from this make 1 source mesh, 1 target mesh, and then use MeshSweep to sweep between the meshes?
        //or is this supposed to be a class?

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points","pts","Points which will create Mesh",GH_ParamAccess.list); //0
            pManager.AddIntegerParameter("U count","u","divide curve in u direction",GH_ParamAccess.item,1); //1
            pManager.AddIntegerParameter("V count","v","divide curve in v direction",GH_ParamAccess.item,1); //2
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("Mesh", "mesh", "Mesh with correct vertices", GH_ParamAccess.item);
            pManager.AddGenericParameter("Mesh", "mesh", "Mesh as breps", GH_ParamAccess.list);
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
            int u = 1;
            int v = 1;

            // get inputs
            DA.GetDataList(0, points); //not ref when points
            DA.GetData(1, ref u);
            DA.GetData(2, ref v);

            var s = NurbsSurface.CreateFromCorners(points[0], points[1], points[2], points[3], 0);
          
            List<Brep> iniMesh = new List<Brep>();

            for (int i = 0; i < u; i++)
            {
                for (int j = 0; j < v; j++)
                {
                    Point3d p1 = new Point3d(s.PointAt(s.Domain(0).Length / u * i, s.Domain(1).Length / v * j));
                    Point3d p2 = new Point3d(s.PointAt(s.Domain(0).Length / u * (i + 1), s.Domain(1).Length / v * j));
                    Point3d p3 = new Point3d(s.PointAt(s.Domain(0).Length / u * (i + 1), s.Domain(1).Length / v * (j + 1)));
                    Point3d p4 = new Point3d(s.PointAt(s.Domain(0).Length / u * i, s.Domain(1).Length / v * (j + 1)));
                    iniMesh.Add(Brep.CreateFromCornerPoints(p1, p2, p3, p4, 0));


                }
            }

            //set outputs
            DA.SetDataList(0, iniMesh);


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
            get { return new Guid("7877BFC0-9563-4387-BA2C-DE5FDA4E2403"); }
        }
    }
}

