using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Master
{
    public class MasterComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MasterComponent()
          : base("RodLength", "leff",
              "Calculating effective length of rod",
              "NTNU", "Master")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Diameter", "d", "diameter input", GH_ParamAccess.item); //kan evnt stå list istedenfor item /0
            pManager.AddNumberParameter("Boltnum", "bnr", "bolt number input", GH_ParamAccess.item); //1
            pManager.AddNumberParameter("Angle", "deg", "Degree to grain", GH_ParamAccess.item); //2
            pManager.AddNumberParameter("Axial force", "N", "[kN]", GH_ParamAccess.item); //3
            pManager.AddNumberParameter("Height cross section", "h", "[mm]", GH_ParamAccess.item); //4 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Effective length", "l", "[mm]", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //declare variables
            double d = 0;
            double n = 0;
            double alpha = 0;
            double F = 0; // F in kN
            double h = 0;
            double fx = 11.92; //MPa
            //var lnum = new List<double>();
            // get inputs
            DA.GetData(0, ref d); //use reference not new
            DA.GetData(1, ref n);
            DA.GetData(2, ref alpha);
            DA.GetData(3, ref F);
            DA.GetData(4, ref h);

            double DegreesToRadians(double degrees)
            {
                return degrees * Math.PI / 180.0;
            }
            double e = 4 * d; //vertical gap between centre of two rods, not sure about value
            double leff = 0; //says that assignment of value is unnecessary, but get error if not assigned?
            double leffMin = 6*d;
            double leffMax = h/2-4*d-e/2;
            leff = (F*1000) * (1.2 * Math.Pow(Math.Cos(DegreesToRadians(alpha)), 2) + (Math.Pow(Math.Sin(DegreesToRadians(alpha)), 2))) / (Math.Pow(n, 0.9) * fx * d); //få inn rho_a/rho_k etterhvert
            //if leff<leffMin -> leff=leffMin, if leff>leffMax -> d+2, run again.

            //set outputs
            DA.SetData(0, leff);


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

