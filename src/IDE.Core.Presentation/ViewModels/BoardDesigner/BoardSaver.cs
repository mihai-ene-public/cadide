using System.Linq;
using IDE.Core.Storage;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;

namespace IDE.Documents.Views;

public class BoardSaver
{
    public void Save(IBoardDesigner board, BoardDocument boardDocument)
    {
        var canvasModel = board.CanvasModel;

        //remove the currently adding item si that it won't be saved
        ISelectableItem placingObject = null;
        if (canvasModel.PlacementTool != null && canvasModel.PlacementTool.CanvasItem != null)
        {
            placingObject = canvasModel.PlacementTool.CanvasItem;
            // placeObjects.ForEach(c => canvasModel.RemoveItem(c));
            canvasModel.RemoveItem(placingObject);
        }

        boardDocument.DocumentWidth = canvasModel.DocumentWidth;
        boardDocument.DocumentHeight = canvasModel.DocumentHeight;

        //plain items
        var plainItems = ((from l in canvasModel.Items.OfType<LayerDesignerItem>()
                           from s in l.Items.OfType<IPlainDesignerItem>()//.Except(l.Items.OfType<PolygonBoardCanvasItem>().Where(p => p.Signal != null))
                           select (LayerPrimitive)(s as BaseCanvasItem).SaveToPrimitive())
                           .Union(canvasModel.Items.OfType<IPlainDesignerItem>()
                                             .Cast<BaseCanvasItem>().Select(d => (LayerPrimitive)d.SaveToPrimitive())))
                          .ToList();

        //add signal items with no signal as plain items
        var undefinedSignalItems = board.CanvasModel.GetItems().OfType<ISignalPrimitiveCanvasItem>().Where(s => s.Signal == null || string.IsNullOrEmpty(s.Signal.Id));
        plainItems.AddRange(undefinedSignalItems.OfType<BoardCanvasItemViewModel>().Except(undefinedSignalItems.OfType<ViaCanvasItem>()).Select(p => (LayerPrimitive)p.SaveToPrimitive()));

        boardDocument.PlainItems = plainItems;

        //footprints
        boardDocument.Components = canvasModel.GetFootprints().Select(d => (BoardComponentInstance)d.SaveToPrimitive()).ToList();

        SaveBoardOutline(board, boardDocument);

        //net classes
        boardDocument.Classes = board.NetClasses.Cast<NetClassBaseItem>().ToList();

        //signals
        SaveNetList(board, boardDocument);

        SaveLayers(board, boardDocument);

        SaveRules(board, boardDocument);

        //PostSave
        if (placingObject != null)
            canvasModel.AddItem(placingObject);
    }

    void SaveBoardOutline(IBoardDesigner board, BoardDocument boardDocument)
    {
        boardDocument.BoardOutline = ((RegionBoardCanvasItem)board.BoardOutline).ToData();
    }

    void SaveNetList(IBoardDesigner board, BoardDocument boardDocument)
    {
        //items without a net
        var nets = board.NetList.Where(n => !string.IsNullOrEmpty(n.Id)).Select(n =>
                                         new BoardNet
                                         {
                                             NetId = n.Id,
                                             Name = n.Name,
                                             ClassId = n.ClassId,
                                             Pads = n.Pads.Select(p => new PadRef { PadNumber = p.Number, FootprintInstanceId = p.FootprintInstanceId }).ToList(),
                                             Items = n.Items.Cast<BaseCanvasItem>().Select(p => (LayerPrimitive)p.SaveToPrimitive()).ToList()
                                         }).ToList();

        boardDocument.Nets = nets;
    }

    public void SaveLayers(IBoardDesigner board, BoardDocument boardDocument)
    {
        boardDocument.Layers = board.LayerItems.Cast<LayerDesignerItem>().Select(l => new Layer
        {
            Id = l.LayerId,
            StackOrder = l.StackOrder,
            Name = l.LayerName,
            Type = l.LayerType,
            Color = l.LayerColor.ToHexString(),
            Material = l.Material,
            DielectricConstant = l.DielectricConstant,
            Thickness = l.Thickness,

            GerberFileName = l.GerberFileName,
            GerberExtension = l.GerberExtension,
            Plot = l.Plot,
            MirrorPlot = l.MirrorPlot
        })//.OrderBy(l => l.Id)
        .ToList();

        //layer groups
        boardDocument.LayerGroups = board.LayerGroups.Cast<LayerGroupDesignerItem>()
            .Where(l => l.IsReadOnly == false)
            .Select(l =>
        new LayerGroup
        {
            Name = l.Name,
            Layers = l.Layers.Select(gl => new LayerRef { Id = gl.LayerId }).ToList()
        }
        ).ToList();

        //drill pairs
        boardDocument.DrillPairs = board.DrillPairs.Select(dp => new LayerPair
        {
            LayerIdStart = dp.LayerStart.LayerId,
            LayerIdEnd = dp.LayerEnd.LayerId
        }).ToList();


        //layer pairs
        boardDocument.LayerPairs = board.LayerPairs.Where(dp => dp.LayerStart != null && dp.LayerEnd != null).Select(dp => new LayerPair
        {
            LayerIdStart = dp.LayerStart.LayerId,
            LayerIdEnd = dp.LayerEnd.LayerId
        }).ToList();
    }

    public void SaveRules(IBoardDesigner board, BoardDocument boardDocument)
    {
        boardDocument.BoardRules = board.Rules.Cast<AbstractBoardRule>().Select(r => r.SaveToBoardRule()).ToList();
    }
}
