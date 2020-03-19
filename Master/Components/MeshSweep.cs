using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Master.Components
{
    public class MeshSweep : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshSweep class.
        /// </summary>
        public MeshSweep()
          : base("MeshSweep", "MSweep",
              "Take in two MeshPoints (source, target) and divisions and Sweep Mesh",
              "NTNU", "Master")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "Points which will create element", GH_ParamAccess.list);
            pManager.AddGenericParameter("MeshPoints (source)", "Ms", "Mesh source", GH_ParamAccess.item);
            pManager.AddGenericParameter("MeshPoints (target)", "Mt", "Mesh target", GH_ParamAccess.item);
            pManager.AddNumberParameter("Divisions", "w", "How many divisions of the mesh", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "el", "Element class", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
        }

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
            get { return new Guid("392a820b-95f5-47a1-8fea-7b7c5327f798"); }
        }
    }
}