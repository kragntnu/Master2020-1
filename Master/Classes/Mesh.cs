using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class Mesh //rather use this than the MeshPoints?
    {
        public int id;
        public List<Element> elements;
        public List<Node> nodes;
        public Mesh()
            {
            }

    }
}
