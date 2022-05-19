using IDE.Core.Errors;
using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    public interface IGeometryHelper
    {

        void GetTextOutlines(ITextCanvasItem text, List<List<XPoint[]>> outlines);

        //void GetTextOutlines(GlobalTextPrimitive text, List<List<XPoint[]>> outlines)

        XRect GetGeometryBounds(ICanvasItem item);
        XRect GetGeometryBounds(object geometry);


        object GetRegionGeometry(IRegionCanvasItem region);

        object GetGeometry(ICanvasItem item, double tolerance = 5e-3, bool applyTransform = false);

        bool Intersects(ICanvasItem item1, ICanvasItem item2);

        bool Intersects(object geometry1, object geometry2);

        object GetIntersection(object geometry, ICanvasItem item);
        object GetIntersection(object geometry1, object geometry2);

        bool ItemIntersectsPoint(ICanvasItem item, XPoint point, double pointDiameter);

        bool ItemIntersectsRectangle(ICanvasItem item, XRect rect);

        void AppendOutlines(object geometry, List<List<XPoint[]>> outlines);

        IList<XPoint> GetOutlinePoints(object geometry);

        CanvasLocation CheckClearance(ICanvasItem item1, ICanvasItem item2, double clearance);
    }

    
}