using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Common.Geometries
{
    public class PolylineGeometryOutline : GeometryOutline
    {
        public PolylineGeometryOutline(IList<XPoint> polylinePoints, double width)
        {
            var outline = GetOutlinedGeometry(polylinePoints, 0.5 * width);

            points.AddRange(outline);
        }
    }
}
