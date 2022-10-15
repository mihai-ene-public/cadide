using IDE.Core.Interfaces;
using IDE.Core.Common.Geometries;

namespace IDE.Core.Designers;

public class PlaneGeometryOutlinePourProcessor : BaseGeometryOutlinePourProcessor, IPlaneGeometryOutlinePourProcessor
{
    protected override GeometryOutline GetGeometry(ISelectableItem item)
    {
        try
        {
            dynamic brdItem = item;
            var brd = brdItem.LayerDocument as IBoardDesigner;
            if (brd != null)
            {
                var brdOutline = brd.BoardOutline;

                var pathGeometry = new PathGeometryOutline(brdOutline.StartPoint);

                foreach (var regionItem in brdOutline.Items)
                {
                    switch (regionItem)
                    {
                        case ILineRegionItem line:
                            pathGeometry.AddLine(line.EndPoint);
                            break;

                        case IArcRegionItem arc:
                            pathGeometry.AddArc(arc.EndPoint, arc.SizeDiameter, arc.SweepDirection);
                            break;
                    }

                }

                return pathGeometry;
            }
        }
        catch
        {
        }
        return null;
    }
}
