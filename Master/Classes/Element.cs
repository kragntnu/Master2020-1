using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Master
{
    class Element
    {
        public int id;
        public Point3d Node1;
        public Point3d Node2;
        public Point3d Node3;
        public Point3d Node4;
        public Point3d Node5;
        public Point3d Node6;
        public Point3d Node7;
        public Point3d Node8;



        public Element ()
        {
            //empty constructor
        }
        public Element(int id1)
        {
            id = id1; //empty constructor
        }
    }
}
