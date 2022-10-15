using System.Collections.Generic;
using ClipperLib;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;

namespace IDE.Core.Common.Geometries
{
    /// <summary>
    /// a collection of outlines that pose as figures; will have zero length points
    /// </summary>
    public class GeometryOutlines : GeometryOutline
    {
        public IList<IGeometryOutline> Outlines { get; set; } = new List<IGeometryOutline>();

        public List<XPoint> GetOutlinePoints(int index)
        {
            var outlinePoints = Outlines[index].GetOutline();

            if (Transform == null)
                return outlinePoints;

            var transformedOutline = new List<XPoint>();

            for (int i = 0; i < outlinePoints.Count; i++)
                transformedOutline.Add(Transform.Transform(outlinePoints[i]));

            return transformedOutline;
        }

        public override XRect GetBounds()
        {
            var rect = new XRect();
            for(int i = 0; i < Outlines.Count; i++)
            {
                var bounds= Outlines[i].GetBounds();
                rect.Union(bounds);
            }

            return rect;
        }
    }
}
