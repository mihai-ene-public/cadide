using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Documents.Views;

namespace IDE.Core.Build;

public class BoardGlobalOutputHelper
{
    public BoardGlobalOutputHelper()
    {
        _globalPrimitiveHelper = new GlobalPrimitiveHelper();
        _boardContextBuilder = new BoardContextBuilder();
    }

    private readonly GlobalPrimitiveHelper _globalPrimitiveHelper;
    private readonly BoardContextBuilder _boardContextBuilder;

    public BuildGlobalResult Build(IBoardDesigner board, LayerType[] validLayerTypes)
    {
        var canvasModel = board.CanvasModel;
        var boardContext = _boardContextBuilder.BuildBoardContext(canvasModel);

        var layers = BuildLayers(board.LayerItems, boardContext, validLayerTypes);
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

    public BuildGlobalResult Build(IFootprintDesigner footprintDesigner, LayerType[] validLayerTypes)
    {
        var canvasModel = footprintDesigner.CanvasModel;
        var boardContext = _boardContextBuilder.BuildBoardContext(canvasModel);

        var rect = GetBoundingRect(boardContext.CanvasItems.Cast<ISelectableItem>());

        const int rectSizeModifier = 3;
        var rectSize = rectSizeModifier * Math.Max(rect.Width, rect.Height);
        var rectCenter = rect.GetCenter();
        var footprintRectangle = new RectangleBoardCanvasItem
        {
            BorderWidth = 0,
            IsFilled = true,
            X = rectCenter.X,
            Y = rectCenter.Y,
            Width = rectSize,
            Height = rectSize
        };

        boardContext.PcbBody = footprintRectangle;

        var docLayers = new List<ILayerDesignerItem>(footprintDesigner.LayerItems);
        //add some extra layers
        docLayers.Add(new LayerDesignerItem(null)
        {
            LayerType = LayerType.Dielectric
        });
        docLayers.Add(new LayerDesignerItem(null)
        {
            LayerType = LayerType.Signal,
            LayerId = LayerConstants.SignalBottomLayerId
        });
        docLayers.Add(new LayerDesignerItem(null)
        {
            LayerType = LayerType.SolderMask,
            LayerId = LayerConstants.SolderBottomLayerId
        });

        var layers = BuildLayers(docLayers, boardContext, validLayerTypes);

        //todo: maybe create a region for this rectangle
        var brdOutline = _globalPrimitiveHelper.GetGlobalPrimitive(footprintRectangle);
        foreach (var layer in layers)
        {
            layer.BoardOutline = brdOutline;
        }

        var bodyRect = footprintRectangle.GetBoundingRectangle();

        return new BuildGlobalResult
        {
            Layers = layers,
            BodyRectangle = bodyRect,
        };
    }

    private XRect GetBoundingRect(IEnumerable<ISelectableItem> canvasItems)
    {
        var rect = XRect.Empty;


        var pads = canvasItems.OfType<IPadCanvasItem>().ToList();

        var rectSource = pads.Count > 0 ? pads : canvasItems;

        //offset all primitives to the center given by the center of the bounding rectange of all primitives
        foreach (BaseCanvasItem item in rectSource)
        {
            var itemRect = item.GetBoundingRectangle();
            rect.Union(itemRect);
        }

        if (rect.IsEmpty)
            return new XRect(0, 0, 5, 5);

        return rect;
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

    private List<BoardGlobalLayerOutput> BuildLayers(IList<ILayerDesignerItem> documentLayers, BoardContext boardContext, LayerType[] validLayerTypes)
    {
        var outputLayers = new List<BoardGlobalLayerOutput>();

        var itemsGroupedByLayer = boardContext.CanvasItems.OfType<SingleLayerBoardCanvasItem>()
                                          .Union(boardContext.FootprintItems.OfType<SingleLayerBoardCanvasItem>())
                                          .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                          .GroupBy(p => p.LayerId);

        var layers = documentLayers.Where(l => validLayerTypes.Contains(l.LayerType)).ToList();

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
