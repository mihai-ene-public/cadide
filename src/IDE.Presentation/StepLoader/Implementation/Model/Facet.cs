using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CADLoader.Implementation.Parser
{
    public class Facet
    {

        public Facet()
        {
            Verticies = new List<Vector3D>();
        }

        public Vector3D Normal { get; set; }
        public IList<Vector3D> Verticies { get; private set; }
    }
}