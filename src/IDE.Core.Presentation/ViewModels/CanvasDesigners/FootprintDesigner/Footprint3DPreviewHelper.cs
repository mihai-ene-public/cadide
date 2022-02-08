using System;
using System.Linq;
using System.Collections.Generic;
using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core;
using IDE.Core.Interfaces;
using System.Threading.Tasks;
using IDE.Core.Types.Media;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Common;


namespace IDE.Documents.Views
{
    public class Footprint3DPreviewHelper
    {
        public Footprint3DPreviewHelper(IDispatcherHelper dispatcher)
        {
            var meshHelper = ServiceProvider.Resolve<IMeshHelper>();
            _meshHelper = meshHelper;
            _dispatcher = dispatcher;
        }
        private readonly IMeshHelper _meshHelper;
        private readonly IDispatcherHelper _dispatcher;

        public Task<BoardPreview3DViewModel> ShowFootprintGeometries(ILayeredViewModel layeredViewModel)
        {
            IList<Preview3DLayerData> solidLayers = new List<Preview3DLayerData>();
            var meshHelper = ServiceProvider.Resolve<IMeshHelper>();

            var canvasModel = layeredViewModel.CanvasModel;

            var canvasItems = canvasModel.GetItems().Select(c => ((BaseCanvasItem)c).Clone()).ToList();

            var solderMaskColor = XColor.FromHexString("#DD008000");

            ////offset all primitives to the center given by the center of the bounding rectange of all primitives
            var rect = OffsetItemsToOrigin(canvasItems.OfType<IPadCanvasItem>().Cast<ISelectableItem>(), canvasItems.Cast<ISelectableItem>());


            var padItems = canvasItems.OfType<IPadCanvasItem>().ToList();
            //drills from pads and holes
            var drillItems = padItems.OfType<PadThtCanvasItem>().Select(
                p => new HoleCanvasItem
                {
                    Drill = p.Drill,
                    X = p.X + p.Hole.X,
                    Y = p.Y + p.Hole.Y,
                    DrillType = p.Hole.DrillType,
                    Rot = p.Hole.Rot,
                    Height = p.Hole.Height
                }).Cast<ICanvasItem>().Union(canvasItems.OfType<HoleCanvasItem>())
           .ToList();

            var millingItems = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                                .Where(c => c.Layer.LayerId == LayerConstants.MultiLayerMillingId)
                                                .OfType<ICanvasItem>().ToList();

            //silk
            var silkItems = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                        .Where(c => c.Layer.LayerId == LayerConstants.SilkscreenTopLayerId)
                                       .OfType<ICanvasItem>().ToList();
            var excludeSilkItems = new List<ICanvasItem>();
            //var excludeSilkItems = padItems.Union(drillItems).Union(millingItems);
            var silkGeometry = meshHelper.BuildMeshFromItems2(silkItems, excludeSilkItems, 0.04, 0.01);
            solidLayers.Add(new Preview3DLayerData { Mesh = silkGeometry, Color = XColors.White });

            //top copper
            var topCopperItems = canvasItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer.LayerId == LayerConstants.SignalTopLayerId).OfType<ICanvasItem>();
            topCopperItems = topCopperItems.Union(padItems);
            var topCopperExclude = drillItems.Union(millingItems);
            var topCopperGeometry = meshHelper.BuildMeshFromItems2(topCopperItems, topCopperExclude, 0.02, 0.01);
            solidLayers.Add(new Preview3DLayerData { Mesh = topCopperGeometry, Color = XColors.Gold });

            //solder mask; thin green stuff with geometries substacted
            var solderMaskItems = canvasItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer.LayerId == LayerConstants.SolderTopLayerId).OfType<ICanvasItem>().ToList();
            //drills from pads
            solderMaskItems.AddRange(drillItems);
            solderMaskItems.AddRange(millingItems);

            solderMaskItems.AddRange(padItems.Where(p => p.AutoGenerateSolderMask));

            //holes and slots
            const int rectSizeModifier = 3;
            var rectSize = rectSizeModifier * Math.Max(rect.Width, rect.Height);
            var footprintRectangle = new RectangleBoardCanvasItem
            {
                BorderWidth = 0,
                IsFilled = true,
                Width = rectSize,
                Height = rectSize
            };
            //solderMaskItems.Add(footprintRectangle);
            var solderMaskGeometry = meshHelper.BuildMeshFromItems2(new[] { footprintRectangle }, solderMaskItems, 0.02, 0.02);
            solidLayers.Add(new Preview3DLayerData { Mesh = solderMaskGeometry, Color = solderMaskColor });

            //paste items
            var pasteMaskItems = canvasItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer.LayerType == LayerType.PasteMask).OfType<ICanvasItem>().ToList();
            //from the pads we get items from the paste mask layer
            pasteMaskItems.AddRange(padItems.Where(p => p.AutoGeneratePasteMask));
            //keep this below: we need to translate the items apropriately
            //pasteMaskItems.AddRange(padItems.Cast<PadBaseCanvasItem>()
            //                                .SelectMany(p => p.Items.OfType<SingleLayerBoardCanvasItem>()
            //                                                .Where(c => c.Layer.LayerId == LayerConstants.PasteTopLayerId)));
            //PasteMaskGeometry = pasteMaskItems.BuildMeshFromItems2(0.02, 0.02);

            //solder body: extract holes, slots, cutouts
            //var pcbBodyItems = new[] { footprintRectangle };
            var drillsSlotsCutoutsitems = new List<ICanvasItem>();
            drillsSlotsCutoutsitems.AddRange(drillItems);
            drillsSlotsCutoutsitems.AddRange(millingItems);
            var pCBBodyGeometry = meshHelper.BuildMeshFromItems2(new[] { footprintRectangle }, drillsSlotsCutoutsitems, -1.52, 1.5);
            solidLayers.Add(new Preview3DLayerData { Mesh = pCBBodyGeometry, Color = XColors.Orange });

            //bottom copper
            var botCopperItems = new List<ICanvasItem>();
            var botCopperExclude = drillItems.Union(millingItems);
            //tht pads
            botCopperItems.AddRange(padItems.OfType<PadThtCanvasItem>());
            solidLayers.Add(new Preview3DLayerData { Mesh = meshHelper.BuildMeshFromItems2(botCopperItems, botCopperExclude, -1.54, 0.02), Color = XColors.Gold });

            //bottom solder mask
            var botSolderMaskItems = new List<ICanvasItem>();
            botSolderMaskItems.AddRange(drillItems);
            botSolderMaskItems.AddRange(millingItems);
            botSolderMaskItems.AddRange(padItems.OfType<PadThtCanvasItem>());
            var botSolderMaskGeometry = meshHelper.BuildMeshFromItems2(new[] { footprintRectangle }, botSolderMaskItems, -1.56, 0.02);
            solidLayers.Add(new Preview3DLayerData { Mesh = botSolderMaskGeometry, Color = solderMaskColor });

            var model = new BoardPreview3DViewModel((ICanvasDesignerFileViewModel)layeredViewModel, _dispatcher)
            {
                Preview3DLayers = solidLayers.OrderByDescending(l => l.Alpha).ToList(),
                // Items = models
            };

            return Task.FromResult(model);
        }

        public async Task<BoardPreview3DViewModel> GeneratePreview(ILayeredViewModel layeredViewModel)
        {
            var boardContext = new BoardContext();

            IList<Preview3DLayerData> solidLayers = new List<Preview3DLayerData>();

            var canvasModel = layeredViewModel.CanvasModel;

            var canvasItems = canvasModel.GetItems().ToList();

            var rect = GetBoundingRect(canvasItems);

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

            var solderMaskColor = XColor.FromHexString("#DD008000");

            boardContext.PadItems = canvasItems.OfType<IPadCanvasItem>().ToList();

            boardContext.DrillItems = boardContext.PadItems.OfType<PadThtCanvasItem>().Select(
                p => new HoleCanvasItem
                {
                    Drill = p.Drill,
                    X = p.X + p.Hole.X,
                    Y = p.Y + p.Hole.Y,
                    DrillType = p.Hole.DrillType,
                    Rot = p.Hole.Rot,
                    Height = p.Hole.Height
                }).Cast<ICanvasItem>().Union(canvasItems.OfType<HoleCanvasItem>())
           .ToList();

            boardContext.MillingItems = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                                .Where(c => c.Layer.LayerId == LayerConstants.MultiLayerMillingId)
                                                .OfType<ICanvasItem>().ToList();

            //layer types: stackup: signal, plane, dielectric;
            //             milling, silkscreen, soldermask, pasteMask 
            var validLayerTypes = new[] { LayerType.Signal,
                                          LayerType.Plane,
                                          LayerType.PasteMask,//?
                                          LayerType.SolderMask,
                                          LayerType.SilkScreen,
                                          LayerType.Dielectric };

            var itemsGroupedByLayer = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                                .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                                .GroupBy(p => p.LayerId);

            var layerStartHeight = 0.0001d;
            var layers = layeredViewModel.LayerItems.Where(l => validLayerTypes.Contains(l.LayerType)).ToList();

            var stackLayers = BuildLayerOrder(layers);

            boardContext.PadItemsOnTop = boardContext.PadItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                                                                 || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId)
                           .ToList();

            boardContext.PadItemsOnBottom = boardContext.PadItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                                                                            || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId)
                                           .ToList();


            foreach (var layer in stackLayers)
            {
                var layerId = layer.LayerId;
                var layerGroup = itemsGroupedByLayer.FirstOrDefault(g => g.Key == layerId);

                var layerThickness = GetDefaultThicknessForLayerType(layer.LayerType);

                switch (layerId)
                {
                    //top silkScreen
                    case LayerConstants.SilkscreenTopLayerId:
                        {
                            var previewLayer = BuildSilkScreenTop(layer, layerGroup, XColors.White, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //top solder mask
                    case LayerConstants.SolderTopLayerId:
                        {
                            layerStartHeight -= layerThickness;
                            var previewLayer = BuildSolderMaskTop(boardContext, layer, layerGroup, solderMaskColor, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //top copper
                    case LayerConstants.SignalTopLayerId:
                        {
                            layerStartHeight -= layerThickness;
                            var previewLayer = BuildSignalTop(boardContext, layer, layerGroup, XColors.Gold, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //bottom copper
                    case LayerConstants.SignalBottomLayerId:
                        {
                            layerStartHeight -= layerThickness;
                            var previewLayer = BuildSignalBottom(boardContext, layer, layerGroup, XColors.Gold, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //bottom mask
                    case LayerConstants.SolderBottomLayerId:
                        {
                            layerStartHeight -= layerThickness;
                            var previewLayer = BuildSolderMaskBottom(boardContext, layer, layerGroup, solderMaskColor, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //bottom silk
                    case LayerConstants.SilkscreenBottomLayerId:
                        {
                            layerStartHeight -= layerThickness;
                            var previewLayer = BuildSilkScreenBottom(layer, layerGroup, XColors.White, layerStartHeight, layerThickness);
                            solidLayers.Add(previewLayer);
                            break;
                        }

                    //rest of the layers
                    default:
                        {
                            //signal, plane, dielectric

                            switch (layer.LayerType)
                            {
                                //inner signal
                                case LayerType.Signal:
                                    {
                                        layerStartHeight -= layerThickness;
                                        var previewLayer = BuildSignalInnerLayers(boardContext, layer, layerGroup, XColors.Gold, layerStartHeight, layerThickness);
                                        solidLayers.Add(previewLayer);
                                        break;
                                    }
                                //plane layer
                                case LayerType.Plane:
                                    {
                                        layerStartHeight -= layerThickness;
                                        var previewLayer = BuildCopperPlane(boardContext, layer, layerGroup, XColors.Gold, layerStartHeight, layerThickness);
                                        solidLayers.Add(previewLayer);
                                        break;
                                    }
                                case LayerType.Dielectric:
                                    {
                                        layerStartHeight -= layerThickness;
                                        var previewLayer = BuildDielecticLayer(boardContext, layer, layerGroup, XColors.Orange, layerStartHeight, layerThickness);
                                        solidLayers.Add(previewLayer);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }

            var model = new BoardPreview3DViewModel((ICanvasDesignerFileViewModel)layeredViewModel, _dispatcher)
            {
                Preview3DLayers = solidLayers.OrderByDescending(l => l.Alpha).ToList(),
                GridWidth = rectSize * 3,
                GridHeight = rectSize * 3
            };

            //offset
            if (footprintRectangle != null)
            {
                var bodyRect = footprintRectangle.GetBoundingRectangle();
                model.OffsetX = -bodyRect.X - 0.5 * bodyRect.Width;
                model.OffsetY = -bodyRect.Y - 0.5 * bodyRect.Height;
            }

            await Task.CompletedTask;

            // model.HandleLayers();
            return model;
        }

        double GetDefaultThicknessForLayerType(LayerType layerType)
        {
            var d = new Dictionary<LayerType, double>
            {
                [LayerType.Signal] = 0.01d,
                [LayerType.SolderMask] = 0.011d,
                [LayerType.Dielectric] = 1.0d,
                [LayerType.SilkScreen] = 0.01d,
                [LayerType.PasteMask] = 0.2d,
            };

            if (d.ContainsKey(layerType))
                return d[layerType];

            return 0.01d;
        }
        private List<ILayerDesignerItem> BuildLayerOrder(List<ILayerDesignerItem> layers)
        {
            //add some extra layers
            layers.Add(new LayerDesignerItem(null)
            {
                LayerType = LayerType.Dielectric
            });
            layers.Add(new LayerDesignerItem(null)
            {
                LayerType = LayerType.Signal,
                LayerId = LayerConstants.SignalBottomLayerId
            });
            layers.Add(new LayerDesignerItem(null)
            {
                LayerType = LayerType.SolderMask,
                LayerId = LayerConstants.SolderBottomLayerId
            });


            var stackLayers = new List<ILayerDesignerItem>();

            #region Build layer order

            //top silk
            var tSilk = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SilkscreenTopLayerId);
            if (tSilk != null)
            {
                stackLayers.Add(tSilk);
                layers.Remove(tSilk);
            }

            //top paste
            var tPaste = layers.FirstOrDefault(l => l.LayerId == LayerConstants.PasteTopLayerId);
            if (tPaste != null)
            {
                stackLayers.Add(tPaste);
                layers.Remove(tPaste);
            }

            //top mask
            var tMask = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SolderTopLayerId);
            if (tMask != null)
            {
                stackLayers.Add(tMask);
                layers.Remove(tMask);
            }

            //top copper
            var tCopper = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SignalTopLayerId);
            if (tCopper != null)
            {
                stackLayers.Add(tCopper);
                layers.Remove(tCopper);
            }

            //stackup inner layers in the order presented in the layers tab in properties: signal, plane, dielectric
            var bottomSideLayerIds = new[]
            {
                LayerConstants.SignalBottomLayerId,
                LayerConstants.PasteBottomLayerId,
                LayerConstants.SolderBottomLayerId,
                LayerConstants.SilkscreenBottomLayerId
            };

            stackLayers.AddRange(layers.Where(l => !bottomSideLayerIds.Contains(l.LayerId)));


            //bottom copper
            var bCopper = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SignalBottomLayerId);
            if (bCopper != null)
            {
                stackLayers.Add(bCopper);
            }

            //bottom paste
            var bPaste = layers.FirstOrDefault(l => l.LayerId == LayerConstants.PasteBottomLayerId);
            if (bPaste != null)
            {
                stackLayers.Add(bPaste);
            }

            //bottom mask
            var bMask = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SolderBottomLayerId);
            if (bMask != null)
            {
                stackLayers.Add(bMask);
            }

            //bottom silk
            var bSilk = layers.FirstOrDefault(l => l.LayerId == LayerConstants.SilkscreenBottomLayerId);
            if (bSilk != null)
            {
                stackLayers.Add(bSilk);
            }

            #endregion
            return stackLayers;
        }

        Preview3DLayerData BuildSilkScreenTop(ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            //exclude removals for now because it creates some artefacts
            var toExcludeItems = new List<ICanvasItem>();
            //excludeSilkItems.AddRange(padItemsOnTop);
            //excludeSilkItems.AddRange(drillItems);
            //excludeSilkItems.AddRange(millingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildSilkScreenBottom(ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            //exclude removals for now because it creates some artefacts
            var toExcludeItems = new List<ICanvasItem>();
            //excludeSilkItems.AddRange(padItemsOnTop);
            //excludeSilkItems.AddRange(drillItems);
            //excludeSilkItems.AddRange(millingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }


        Preview3DLayerData BuildSolderMaskTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            toAddItems.Add(boardContext.PcbBody);

            if (itemsOnLayer != null)
                toExcludeItems.AddRange(itemsOnLayer.OfType<ICanvasItem>());
            //drills from pads
            toExcludeItems.AddRange(boardContext.DrillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || (d.ParentObject as ViaCanvasItem)?.TentViaOnTop == false));
            toExcludeItems.AddRange(boardContext.MillingItems);
            toExcludeItems.AddRange(boardContext.PadItemsOnTop.Where(p => p.AutoGenerateSolderMask));
            toExcludeItems.AddRange(boardContext.ViaItems.Where(v => v.TentViaOnTop == false));
            //tht pads
            toExcludeItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildSolderMaskBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            toAddItems.Add(boardContext.PcbBody);

            if (itemsOnLayer != null)
                toExcludeItems.AddRange(itemsOnLayer.OfType<ICanvasItem>());
            //drills from pads
            toExcludeItems.AddRange(boardContext.DrillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || (d.ParentObject as ViaCanvasItem)?.TentViaOnBottom == false));
            toExcludeItems.AddRange(boardContext.MillingItems);
            toExcludeItems.AddRange(boardContext.PadItemsOnBottom.Where(p => p.AutoGenerateSolderMask));
            toExcludeItems.AddRange(boardContext.ViaItems.Where(v => v.TentViaOnBottom == false));
            //tht pads
            toExcludeItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }


        Preview3DLayerData BuildSignalTop(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            //smd pads on top
            toAddItems.AddRange(boardContext.PadItemsOnTop.OfType<PadSmdCanvasItem>());

            //tht pads
            toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

            //via pads on Top
            toAddItems.AddRange(boardContext.ViaItems);

            toExcludeItems.AddRange(boardContext.DrillItems);
            toExcludeItems.AddRange(boardContext.MillingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildSignalBottom(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            //smd pads on bottom
            toAddItems.AddRange(boardContext.PadItemsOnBottom);

            //tht pads
            toAddItems.AddRange(boardContext.PadItems.OfType<PadThtCanvasItem>());

            //via pads on Top
            toAddItems.AddRange(boardContext.ViaItems);

            toExcludeItems.AddRange(boardContext.DrillItems);
            toExcludeItems.AddRange(boardContext.MillingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildSignalInnerLayers(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            toExcludeItems.AddRange(boardContext.DrillItems);
            toExcludeItems.AddRange(boardContext.MillingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildCopperPlane(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            if (itemsOnLayer != null)
                toAddItems.AddRange(itemsOnLayer.ToList());

            toExcludeItems.AddRange(boardContext.DrillItems);
            toExcludeItems.AddRange(boardContext.MillingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData BuildDielecticLayer(BoardContext boardContext, ILayerDesignerItem layer, IEnumerable<ICanvasItem> itemsOnLayer, XColor color, double layerStartHeight, double layerThickness)
        {
            var toAddItems = new List<ICanvasItem>();
            var toExcludeItems = new List<ICanvasItem>();

            toAddItems.Add(boardContext.PcbBody);

            toExcludeItems.AddRange(boardContext.DrillItems);
            toExcludeItems.AddRange(boardContext.MillingItems);

            var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, layerStartHeight, layerThickness);

            return previewLayer;
        }

        Preview3DLayerData CreateLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> toAddItems, IEnumerable<ICanvasItem> toExcludeItems, XColor color, double layerStartHeight, double layerThickness)
        {
            var mesh = _meshHelper.BuildMeshFromItems2(toAddItems, toExcludeItems, layerStartHeight, layerThickness);
            return CreateLayer(mesh, color, layer.LayerColor, layer.LayerType, layerStartHeight);
        }
        private Preview3DLayerData CreateLayer(IMeshModel mesh, XColor color, XColor layerColor, LayerType layerType, double posZ)
        {
            return new Preview3DLayerData
            {
                Mesh = mesh,
                Color = color,
                DisplayColor = color,
                LayerColor = layerColor,
                LayerType = layerType,
                PositionZ = posZ
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

        XRect OffsetItemsToOrigin(IEnumerable<ISelectableItem> canvasItemsRectangle, IEnumerable<ISelectableItem> canvasItems)
        {
            var rect = XRect.Empty;

            //offset all primitives to the center given by the center of the bounding rectange of all primitives
            var xOffset = double.MaxValue;
            var yOffset = double.MaxValue;
            foreach (BaseCanvasItem item in canvasItemsRectangle)
            {
                var itemRect = item.GetBoundingRectangle();

                if (xOffset > itemRect.X)
                    xOffset = itemRect.X;
                if (yOffset > itemRect.Y)
                    yOffset = itemRect.Y;

                rect.Union(itemRect);
            }
            if (rect != XRect.Empty)
            {
                //offset relative to the center of the rectangle
                foreach (BaseCanvasItem item in canvasItems)
                    item.Translate(-xOffset - rect.Width * 0.5, -yOffset - rect.Height * 0.5);
            }

            return rect;
        }

    }
}
