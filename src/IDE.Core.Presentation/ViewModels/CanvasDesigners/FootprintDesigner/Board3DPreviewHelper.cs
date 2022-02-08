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
    public class Board3DPreviewHelper
    {
        public Board3DPreviewHelper(IDispatcherHelper dispatcher)
        {
            var meshHelper = ServiceProvider.Resolve<IMeshHelper>();
            _meshHelper = meshHelper;
            _dispatcher = dispatcher;
        }
        private readonly IMeshHelper _meshHelper;
        private readonly IDispatcherHelper _dispatcher;

        public async Task<BoardPreview3DViewModel> ShowBoardGeometries(IBoardDesigner layeredViewModel, ISolutionProjectNodeModel parentProject, double boardTotalThickness)
        {
            //todo: case for outline inside another outline (for fills)

            var boardContext = new BoardContext();

            IList<Preview3DLayerData> solidLayers = new List<Preview3DLayerData>();

            var canvasModel = layeredViewModel.CanvasModel;

            var canvasItems = canvasModel.GetItems().ToList();

            boardContext.PcbBody = canvasItems.OfType<RegionBoardCanvasItem>().FirstOrDefault();

            //smd and pad
            var footprints = canvasItems.OfType<FootprintBoardCanvasItem>().ToList();
            var footprintItems = (from fp in footprints
                                  from p in fp.Items
                                  select p
                                  ).ToList();

            //we add designators as text items
            var designators = (from fp in footprints
                               where fp.ShowName
                               select (BaseCanvasItem)fp.Designator)
                              .ToList();

            footprintItems.AddRange(designators);

            boardContext.PadItems = footprintItems.OfType<IPadCanvasItem>().ToList();
            boardContext.PadItems.AddRange(canvasItems.OfType<IPadCanvasItem>());
            boardContext.ViaItems = canvasItems.OfType<ViaCanvasItem>().ToList();

            //todo: we must assign a layer for the drill pair; for now, top and bottom

            //layer types: stackup: signal, plane, dielectric;
            //             milling, silkscreen, soldermask, pasteMask 
            var validLayerTypes = new[] { LayerType.Signal,
                                          LayerType.Plane,
                                          LayerType.PasteMask,//?
                                          LayerType.SolderMask,
                                          LayerType.SilkScreen,
                                          LayerType.Dielectric };

            var solderMaskColor = XColor.FromHexString("#DD008000");

            //drills from pads and holes
            boardContext.DrillItems = boardContext.PadItems.OfType<PadThtCanvasItem>().Select(
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
            )
            .Cast<ICanvasItem>()
            .Union(footprintItems.OfType<HoleCanvasItem>()) //holes from footprints
            .Union(canvasItems.OfType<HoleCanvasItem>())
            .Union(boardContext.ViaItems.Select(v => new CircleBoardCanvasItem
            {
                BorderWidth = 0,
                IsFilled = true,
                Diameter = v.Drill,
                X = v.X,
                Y = v.Y,
                ParentObject = v
            }))
           .ToList();

            boardContext.MillingItems = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                    .Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId)
                                    .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId))
                                    .OfType<ICanvasItem>().ToList();

            var itemsGroupedByLayer = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                                .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>())
                                                .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                                .GroupBy(p => p.LayerId);

            var layerStartHeight = 0.04d;
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

                var layerThickness = 0.01;
                if (layer.Thickness > 0.0)
                    layerThickness = layer.Thickness;


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

            var previewModel = new BoardPreview3DViewModel((ICanvasDesignerFileViewModel)layeredViewModel, _dispatcher);

            //offset
            var brdOutline = layeredViewModel.BoardOutline;
            if (brdOutline != null)
            {
                var bodyRect = brdOutline.GetBoundingRectangle();
                previewModel.OffsetX = -bodyRect.X - 0.5 * bodyRect.Width;
                previewModel.OffsetY = -bodyRect.Y - 0.5 * bodyRect.Height;
            }

            var models = new List<ISelectableItem>();

            foreach (var fp in footprints)
            {
                await LoadModelForFootprint(fp, models, boardTotalThickness, parentProject, previewModel);
            }

            previewModel.Preview3DLayers = solidLayers;//.OrderByDescending(l => l.Alpha).ToList();
            previewModel.Items = models;

            return previewModel;
        }

        private List<ILayerDesignerItem> BuildLayerOrder(List<ILayerDesignerItem> layers)
        {
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

        async Task LoadModelForFootprint(FootprintBoardCanvasItem fp, List<ISelectableItem> models, double boardTotalThickness,
                                        ISolutionProjectNodeModel parentProject, BoardPreview3DViewModel previewModel)
        {
            //todo: we need to calculate a transform of the model of the footprint
            if (fp == null) return;
            var footprint = fp.CachedFootprint;
            if (footprint == null || footprint.Models == null)
                return;
            foreach (var modelData in footprint.Models)
            {
                //if model is local to footprint, then it is in the same library as footprint
                string modelLibrary = null;
                if (modelData.ModelLibrary == null || modelData.ModelLibrary == "local")
                    modelLibrary = footprint.Library;

                var modelDoc = await Task.Run(() => parentProject.FindObject(TemplateType.Model, modelLibrary, modelData.ModelId) as ModelDocument);
                // var direction = -2.0;
                var height = 0.04;
                var posZ = modelData.CenterZ;
                if (fp.Placement == FootprintPlacement.Bottom)
                {
                    height = -boardTotalThickness;
                    posZ = -posZ;
                }

                var t = new XTransformGroup();
                t.Children.Add(new XRotateTransform(fp.Rot));
                if (fp.Placement == FootprintPlacement.Bottom)
                    t.Children.Add(new XScaleTransform { ScaleX = -1 });

                t.Children.Add(new XTranslateTransform(fp.X, fp.Y));
                t.Children.Add(new XTranslateTransform(previewModel.OffsetX, previewModel.OffsetY));

                if (modelDoc != null && modelDoc.Items != null)
                {
                    var modelPos = new XPoint(modelData.CenterX, modelData.CenterY);
                    modelPos = t.Transform(modelPos);

                    var groupItem = new GroupMeshItem();
                    groupItem.X = modelPos.X;
                    groupItem.Y = modelPos.Y;
                    groupItem.Z = posZ + height;
                    groupItem.RotationX = modelData.RotationX;
                    groupItem.RotationY = modelData.RotationY;
                    var sign = fp.Placement == FootprintPlacement.Bottom ? -1.0 : 1.0;
                    groupItem.RotationZ = sign * (fp.Rot + modelData.RotationZ);
                    if (fp.Placement == FootprintPlacement.Bottom)
                        groupItem.RotationY = ((int)groupItem.RotationY + 180) % 360;

                    groupItem.IsPlaced = true;

                    groupItem.Items.AddRange(modelDoc.Items.Select(item => (BaseMeshItem)item.CreateDesignerItem()));
                    foreach (var item in groupItem.Items)
                        item.ParentObject = groupItem;

                    models.Add(groupItem);
                }
            }
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


    }
}
