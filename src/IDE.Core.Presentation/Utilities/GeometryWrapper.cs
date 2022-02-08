using IDE.Core.Interfaces;

namespace IDE.Core.Presentation
{
    public class GeometryWrapper : IGeometry
    {
        public GeometryWrapper(object geometry)
        {
            Geometry = geometry;
        }

        public object Geometry { get; private set; }
    }
}
