using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Storage;
using IDE.Documents.Views;

namespace IDE.Core.Build;

public class BoardGlobalOutputHelper
{
    public BoardGlobalOutputHelper()
    {
        _globalPrimitiveHelper = new GlobalPrimitiveHelper();
    }

    private readonly GlobalPrimitiveHelper _globalPrimitiveHelper;

    public BuildGlobalResult Build(IBoardDesigner board, LayerType[] validLayerTypes)
    {
        var boardContext = BuildBoardContext(board);

        var layers = BuildLayers(board, boardContext, validLayerTypes);
        var drillPairs = BuildDrillPairs(board, boardContext);

        var brdOutline = _globalPrimitiveHelper.GetGlobalPrimitive(board.BoardOutline);
        foreach (var layer in layers)
        {
            layer.BoardOutline = brdOutline;
        }

        return new BuildGlobalResult
        {
            Layers = layers,
            DrillLayers = drillPairs
        };
    }

    private IList<GlobalPrimitive> GetPrimitives(IEnumerable<ICanvasItem> canvasItems)
    {
        var globalPrimitives = new List<GlobalPrimitive>();
        foreach (var item in canvasItems)
        {
            var p = _globalPrimitiveHelper.GetGlobalPrimitive(item);
            if (p != null)
            {
                globalPrimitives.Add(p);
            }
        }

        return globalPrimitives;
    }

    private IList<BoardGlobalDrillPairOutput> BuildDrillPairs(IBoardDesigner board, BoardContext boardContext)
    {
        var drillLayers = new List<BoardGlobalDrillPairOutput>();

        var topBottomPair = board.DrillPairs.FirstOrDefault(d => d.LayerStart.LayerId == LayerConstants.SignalTopLayerId
                                                                 && d.LayerEnd.LayerId == LayerConstants.SignalBottomLayerId);

        var drillPrimitives = GetPrimitives(boardContext.DrillItems.Cast<ICanvasItem>());
        

        var millPrimitives = GetPrimitives(boardContext.MillingItems);

        //top-bottom
        drillLayers.Add(new BoardGlobalDrillPairOutput
        {
            DrillPair = topBottomPair,
            DrillPrimitives = drillPrimitives,
            MillingPrimitives = millPrimitives
        });

        //the rest of pairs

        //vias grouped by pair but not top-bottom
        var viasGrouped = boardContext.ViaItems.Where(v => v.DrillPair != topBottomPair).GroupBy(v => v.DrillPair);

        foreach (var viaGroup in viasGrouped)
        {
            var viaHoles = viaGroup.Select(v => new HoleCanvasItem
            {
                Drill = v.Drill,
                X = v.X,
                Y = v.Y
            } as IHoleCanvasItem)
            .ToList();

            var viaHolesPrimitives = GetPrimitives(viaHoles);

            drillLayers.Add(new BoardGlobalDrillPairOutput
            {
                DrillPair = viaGroup.Key,
                DrillPrimitives = viaHolesPrimitives,
            });
        }

        return drillLayers;
    }

    private BoardContext BuildBoardContext(IBoardDesigner board)
    {
        var boardContext = new BoardContext();

        var canvasModel = board.CanvasModel;

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
    private List<BoardGlobalLayerOutput> BuildLayers(IBoardDesigner board, BoardContext boardContext, LayerType[] validLayerTypes)
    {
        var outputLayers = new List<BoardGlobalLayerOutput>();

        var itemsGroupedByLayer = boardContext.CanvasItems.OfType<SingleLayerBoardCanvasItem>()
                                          .Union(boardContext.FootprintItems.OfType<SingleLayerBoardCanvasItem>())
                                          .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                          .GroupBy(p => p.LayerId);

        var layers = board.LayerItems.Where(l => validLayerTypes.Contains(l.LayerType)).ToList();

        foreach (var layer in layers)
        {
            var layerId = layer.LayerId;
            var layerGroup = itemsGroupedByLayer.FirstOrDefault(g => g.Key == layerId);

            switch (layerId)
            {
                //top silkScreen
                case LayerConstants.SilkscreenTopLayerId:
                    {
                        outputLayers.Add(BuildSilkScreenTop(boardContext, layer, layerGroup));
                        break;
                    }

                //bottom silkScreen
                case LayerConstants.SilkscreenBottomLayerId:
                    {
                        outputLayers.Add(BuildSilkScreenBottom(boardContext, layer, layerGroup));
                        break;
                    }

                //top solder mask
                case LayerConstants.SolderTopLayerId:
                    {
                        outputLayers.Add(BuildSolderMaskTop(boardContext, layer, layerGroup));
                        break;
                    }

                //bottom solder mask
                case LayerConstants.SolderBottomLayerId:
                    {
                        outputLayers.Add(BuildSolderMaskBottom(boardContext, layer, layerGroup));
                        break;
                    }

                //top paste mask
                case LayerConstants.PasteTopLayerId:
                    {
                        outputLayers.Add(BuildPasteMaskTop(boardContext, layer, layerGroup));
                        break;
                    }

                //bottom paste mask
                case LayerConstants.PasteBottomLayerId:
                    {
                        outputLayers.Add(BuildPasteMaskBottom(boardContext, layer, layerGroup));
                        break;
                    }

                //top copper
                case LayerConstants.SignalTopLayerId:
                    {
                        outputLayers.Add(BuildSignalTop(boardContext, layer, layerGroup));
                        break;
                    }

                //bottom copper
                case LayerConstants.SignalBottomLayerId:
                    {
                        outputLayers.Add(BuildSignalBottom(boardContext, layer, layerGroup));
                        break;
                    }
                default:
                    {
                        switch (layer.LayerType)
                        {
                            //inner signal
                            case LayerType.Signal:
                                {
                                    outputLayers.Add(BuildSignalInnerLayers(boardContext, layer, layerGroup));
                                    break;
                                }
                            //plane layer
                            case LayerType.Plane:
                                {
                                    outputLayers.Add(BuildCopperPlane(boardContext, layer, layerGroup));
                                    break;
                                }
                            case LayerType.Dielectric:
                                {
                                    outputLayers.Add(BuildDielectricLayer(boardContext, layer, layerGroup));
                                    break;
                                }
                            case LayerType.Mechanical:
                                {
                                    outputLayers.Add(BuildMechanicalLayer(boardContext, layer, layerGroup));
                                    break;
                                }

                            case LayerType.Generic:
                                {
                                    outputLayers.Add(BuildGenericLayer(boardContext, layer, layerGroup));
                                    break;
                                }

                            default:
                                {
                                    outputLayers.Add(BuildGenericLayer(boardContext, layer, layerGroup));
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        return outputLayers;
    }

    private BoardGlobalLayerOutput BuildMechanicalLayer(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildGenericLayer(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildPasteMaskTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);

        toAddItems.AddRange(boardContext.PadItemsOnTop.Where(p => p.AutoGeneratePasteMask));
        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        return GetOutput(layer, toAddItems, toExcludeItems);
    }
    private BoardGlobalLayerOutput BuildPasteMaskBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);

        toAddItems.AddRange(boardContext.PadItemsOnBottom.Where(p => p.AutoGeneratePasteMask));
        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildSolderMaskTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);
        //drills from pads
        toAddItems.AddRange(boardContext.DrillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || ( d.ParentObject as ViaCanvasItem )?.TentViaOnTop == false));
        toAddItems.AddRange(boardContext.MillingItems);
        toAddItems.AddRange(boardContext.PadItemsOnTop.Where(p => p.AutoGenerateSolderMask));
        toAddItems.AddRange(boardContext.ViaItems.Where(v => v.TentViaOnTop == false));
        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        return GetOutput(layer, toAddItems, toExcludeItems);
    }
    private BoardGlobalLayerOutput BuildSolderMaskBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer);
        //drills from pads
        toAddItems.AddRange(boardContext.DrillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || ( d.ParentObject as ViaCanvasItem )?.TentViaOnBottom == false));
        toAddItems.AddRange(boardContext.MillingItems);
        toAddItems.AddRange(boardContext.PadItemsOnBottom.Where(p => p.AutoGenerateSolderMask));
        toAddItems.AddRange(boardContext.ViaItems.Where(v => v.TentViaOnBottom == false));
        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildSignalTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
        {
            foreach (var item in itemsOnLayer)
            {
                if (item is IPolygonCanvasItem poly && poly.PolygonType == PolygonType.Keepout)
                {
                    continue;
                }
                toAddItems.Add(item);
            }
        }

        //smd pads on top
        toAddItems.AddRange(boardContext.PadItemsOnTop.OfType<PadSmdCanvasItem>());

        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        //via pads on Top
        toAddItems.AddRange(boardContext.ViaItems);

        toExcludeItems.AddRange(boardContext.DrillItems);
        toExcludeItems.AddRange(boardContext.MillingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }
    private BoardGlobalLayerOutput BuildSignalBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
        {
            foreach (var item in itemsOnLayer)
            {
                if (item is IPolygonCanvasItem poly && poly.PolygonType == PolygonType.Keepout)
                {
                    continue;
                }
                toAddItems.Add(item);
            }
        }

        //smd pads on bottom
        toAddItems.AddRange(boardContext.PadItemsOnBottom);

        //tht pads
        toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

        //via pads on Top
        toAddItems.AddRange(boardContext.ViaItems);

        toExcludeItems.AddRange(boardContext.DrillItems);
        toExcludeItems.AddRange(boardContext.MillingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }
    private BoardGlobalLayerOutput BuildSignalInnerLayers(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
        {
            foreach (var item in itemsOnLayer)
            {
                if (item is IPolygonCanvasItem poly && poly.PolygonType == PolygonType.Keepout)
                {
                    continue;
                }
                toAddItems.Add(item);
            }
        }

        toExcludeItems.AddRange(boardContext.DrillItems);
        toExcludeItems.AddRange(boardContext.MillingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildCopperPlane(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        if (itemsOnLayer != null)
        {
            foreach (var item in itemsOnLayer)
            {
                if (item is IPolygonCanvasItem poly && poly.PolygonType == PolygonType.Keepout)
                {
                    continue;
                }
                toAddItems.Add(item);
            }
        }

        toExcludeItems.AddRange(boardContext.DrillItems);
        toExcludeItems.AddRange(boardContext.MillingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildDielectricLayer(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        var toExcludeItems = new List<ICanvasItem>();

        toAddItems.Add(boardContext.PcbBody);

        toExcludeItems.AddRange(boardContext.DrillItems);
        toExcludeItems.AddRange(boardContext.MillingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildSilkScreenTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer.ToList());

        //exclude removals for now because it creates some artefacts
        var toExcludeItems = new List<ICanvasItem>();
        //excludeSilkItems.AddRange(padItemsOnTop);
        //excludeSilkItems.AddRange(drillItems);
        //excludeSilkItems.AddRange(millingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput BuildSilkScreenBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer)
    {
        var toAddItems = new List<ICanvasItem>();
        if (itemsOnLayer != null)
            toAddItems.AddRange(itemsOnLayer.ToList());

        //exclude removals for now because it creates some artefacts
        var toExcludeItems = new List<ICanvasItem>();
        //excludeSilkItems.AddRange(padItemsOnTop);
        //excludeSilkItems.AddRange(drillItems);
        //excludeSilkItems.AddRange(millingItems);

        return GetOutput(layer, toAddItems, toExcludeItems);
    }

    private BoardGlobalLayerOutput GetOutput(ILayerDesignerItem layer, IList<ICanvasItem> toAddItems, IList<ICanvasItem> toExcludeItems)
    {
        var addPrimitives = new List<GlobalPrimitive>();
        var excludePrimitives = new List<GlobalPrimitive>();

        foreach (var item in toAddItems)
        {
            var primitive = _globalPrimitiveHelper.GetGlobalPrimitive(item);
            if (primitive != null)
            {
                addPrimitives.Add(primitive);
            }
        }

        foreach (var item in toExcludeItems)
        {
            var primitive = _globalPrimitiveHelper.GetGlobalPrimitive(item);
            if (primitive != null)
            {
                excludePrimitives.Add(primitive);
            }
        }

        return new BoardGlobalLayerOutput
        {
            Layer = layer,
            AddItems = addPrimitives,
            ExtractItems = excludePrimitives,
        };
    }
}
