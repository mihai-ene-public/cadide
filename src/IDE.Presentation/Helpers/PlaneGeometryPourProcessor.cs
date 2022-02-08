using IDE.Core.Interfaces;
using System.Windows.Media;

namespace IDE.Core.Designers
{
    public class PlaneGeometryPourProcessor : BaseGeometryPourProcessor, IPlaneGeometryPourProcessor
    {
        protected override Geometry GetGeometry(ISelectableItem item)
        {
            var brdItem = item as BoardCanvasItemViewModel;

            if (brdItem != null)
            {
                var brd = brdItem.LayerDocument as IBoardDesigner;
                if (brd != null)
                {
                    return (Geometry)GeometryHelper.GetRegionGeometry(brd.BoardOutline);
                }
            }

            return Geometry.Empty;
        }
    }
}
