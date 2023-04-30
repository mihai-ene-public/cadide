using IDE.Core.Designers;
using IDE.Core.Types.Media;
using IDE.Core;

namespace IDE.Documents.Views;

public class BoardOutlineUpdater
{
   public static void UpdateBoardOutline(RegionBoardCanvasItem boardOutlineItem, IList<SingleLayerBoardCanvasItem> outlineItems, int boardOutlineLayerId)
    {
        //we don't want to clear outline if we don't have anything
        if (outlineItems.Count == 0)
            return;

        boardOutlineItem.LayerId = boardOutlineLayerId;
        boardOutlineItem.Items.Clear();

        List<BaseRegionItem> regionItems = new List<BaseRegionItem>();
        XPoint startPoint;
        XPoint endPoint = new XPoint();
        var first = true;

        while (outlineItems.Count > 0)
        {
            SingleLayerBoardCanvasItem item = null;

            if (first)
            {
                item = outlineItems[0];
                outlineItems.RemoveAt(0);
                first = false;
            }
            else
            {
                foreach (var sItem in outlineItems)
                {
                    if (sItem is ArcBoardCanvasItem)
                    {
                        var sArc = sItem as ArcBoardCanvasItem;
                        if (sArc.StartPoint == endPoint || sArc.EndPoint == endPoint)
                        {
                            item = sItem;
                            break;
                        }
                    }
                    else if (sItem is LineBoardCanvasItem)
                    {
                        var sLine = sItem as LineBoardCanvasItem;
                        if (new XPoint(sLine.X1, sLine.Y1) == endPoint || new XPoint(sLine.X2, sLine.Y2) == endPoint)
                        {
                            item = sItem;
                            break;
                        }
                    }
                }

                //we stop if we didnt find an item
                if (item == null)
                    return;

                outlineItems.Remove(item);
            }

            BaseRegionItem regionItem = null;
            var cSp = new XPoint();
            var cEp = new XPoint();
            if (item is ArcBoardCanvasItem)
            {
                var arc = item as ArcBoardCanvasItem;
                cSp = arc.StartPoint;
                cEp = arc.EndPoint;
                regionItem = new ArcRegionItem
                {
                    EndPointX = cEp.X,
                    EndPointY = cEp.Y,
                    IsLargeArc = arc.IsLargeArc,
                    SizeDiameter = arc.Radius,
                    SweepDirection = arc.SweepDirection,
                };
            }
            else if (item is LineBoardCanvasItem)
            {
                var line = item as LineBoardCanvasItem;
                cSp = new XPoint(line.X1, line.Y1);
                cEp = new XPoint(line.X2, line.Y2);
                regionItem = new LineRegionItem { EndPointX = cEp.X, EndPointY = cEp.Y };
            }

            if (regionItems.Count == 0)
            {
                startPoint = cSp;
                endPoint = cEp;
                boardOutlineItem.StartPointX = cSp.X;
                boardOutlineItem.StartPointY = cSp.Y;
                regionItems.Add(regionItem);
                //regionCanvasItems.Add(item);
            }
            else
            {
                if (cSp == endPoint)//current Startpoint is last end point
                    endPoint = cEp;
                else
                {
                    endPoint = cSp;
                    if (regionItem is ArcRegionItem)
                    {
                        //revert
                        var arcRegion = regionItem as ArcRegionItem;
                        arcRegion.SweepDirection = (XSweepDirection)(1 - (int)arcRegion.SweepDirection);
                    }
                }

                regionItem.EndPointX = endPoint.X;
                regionItem.EndPointY = endPoint.Y;
                regionItems.Add(regionItem);
                //regionCanvasItems.Add(item);
            }

        }

        boardOutlineItem.Items.AddRange(regionItems);
        boardOutlineItem.OnPropertyChanged(nameof(boardOutlineItem.Items));
    }
}
