using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Documents.Views;

namespace IDE.Core.Build;

public class BoardContextBuilder
{
    public BoardContext BuildBoardContext(ICanvasDesignerFileViewModel canvasModel)
    {
        var boardContext = new BoardContext();

        var canvasItems = canvasModel.GetItems().ToList();
        boardContext.CanvasItems.AddRange(canvasItems);

        boardContext.PcbBody = canvasItems.OfType<RegionBoardCanvasItem>().FirstOrDefault();

        //smd and pad
        var footprints = canvasItems.OfType<FootprintBoardCanvasItem>().ToList();
        var footprintItems = ( from fp in footprints
                               from p in fp.Items
                               select p
                              ).ToList();

        //we add designators as text items
        var designators = ( from fp in footprints
                            where fp.ShowName
                            select (BaseCanvasItem)fp.Designator )
                          .ToList();

        footprintItems.AddRange(designators);

        boardContext.FootprintItems.AddRange(footprintItems);

        boardContext.PadItems.AddRange(footprintItems.OfType<IPadCanvasItem>());
        boardContext.PadItems.AddRange(canvasItems.OfType<IPadCanvasItem>());
        boardContext.ViaItems.AddRange(canvasItems.OfType<ViaCanvasItem>());

        //todo: we must assign a layer for the drill pair; for now, top and bottom

        //drills from pads and holes
        boardContext.DrillItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>().Select(
        p => new HoleCanvasItem
        {
            Drill = p.Drill,
            X = p.X + p.Hole.X,
            Y = p.Y + p.Hole.Y,
            DrillType = p.Hole.DrillType,
            Rot = p.Hole.Rot,
            Height = p.Hole.Height,
            ParentObject = p.ParentObject
        }
        ));

        boardContext.DrillItems.AddRange(footprintItems.OfType<HoleCanvasItem>()); //holes from footprints
        boardContext.DrillItems.AddRange(canvasItems.OfType<HoleCanvasItem>());
        //vias from top-bottom
        boardContext.DrillItems.AddRange(boardContext.ViaItems.Where(v => v.DrillPair?.LayerStart?.LayerId == LayerConstants.SignalTopLayerId
                                    && v.DrillPair.LayerEnd?.LayerId == LayerConstants.SignalBottomLayerId)
            .Select(v => new HoleCanvasItem
            {
                Drill = v.Drill,
                X = v.X,
                Y = v.Y,
                ParentObject = v
            }));

        boardContext.MillingItems.AddRange(canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                .Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId));
        boardContext.MillingItems.AddRange(footprintItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId));

        boardContext.PadItemsOnTop.AddRange(boardContext.PadItems.Where(p => ( ( (BaseCanvasItem)p ).ParentObject as FootprintBoardCanvasItem )?.Placement == FootprintPlacement.Top
                                                                           || ( (SingleLayerBoardCanvasItem)p ).LayerId == LayerConstants.SignalTopLayerId));

        boardContext.PadItemsOnBottom.AddRange(boardContext.PadItems.Where(p => ( ( (BaseCanvasItem)p ).ParentObject as FootprintBoardCanvasItem )?.Placement == FootprintPlacement.Bottom
                                                                        || ( (SingleLayerBoardCanvasItem)p ).LayerId == LayerConstants.SignalBottomLayerId));

        return boardContext;
    }

}
