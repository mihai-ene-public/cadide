using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;

namespace IDE.Core.Common.Geometries
{

    //arc - thickness

    //polygon
    public class PolygonGeometryOutline : GeometryOutline
    {
        public PolygonGeometryOutline(IList<XPoint> pointsOutline)
        {
            points.AddRange(pointsOutline);
        }
    }
}
