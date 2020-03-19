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
    class Node
    {
        public int id;
        public Point3d location;
        public Node()
        {

        }
        public Node(int id1, Point3d location1)
        {
            id = id1;
            location = location1;
        }

    }
}
