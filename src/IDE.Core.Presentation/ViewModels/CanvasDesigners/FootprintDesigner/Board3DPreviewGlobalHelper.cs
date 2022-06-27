using IDE.Core;
using IDE.Core.Build;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Storage;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views;

public class Board3DPreviewGlobalHelper
{
    public Board3DPreviewGlobalHelper(IDispatcherHelper dispatcher)
    {
        _meshHelper = ServiceProvider.Resolve<IMeshHelper>();
        _objectFinder = ServiceProvider.Resolve<IObjectFinder>();
        _dispatcher = dispatcher;
    }

    private readonly IMeshHelper _meshHelper;
    private readonly IDispatcherHelper _dispatcher;
    private readonly IObjectFinder _objectFinder;

    private readonly Dictionary<LayerType, XColor> _layerTypeColors = new Dictionary<LayerType, XColor>
    {
        { LayerType.SilkScreen, XColors.White},
        { LayerType.SolderMask, XColor.FromHexString("#DD008000")},
        { LayerType.Signal, XColors.Gold},
        { LayerType.Plane, XColors.Gold},
        { LayerType.Dielectric, XColors.Orange},
    };
    public async Task<BoardPreview3DViewModel> GeneratePreview(IBoardDesigner board, double boardTotalThickness)
    {
        var validLayerTypes = new[] { LayerType.Signal,
                                      LayerType.Plane,
                                      LayerType.SolderMask,
                                      LayerType.SilkScreen,
                                      LayerType.Dielectric };

        var outputHelper = new BoardGlobalOutputHelper();
        var buildResult = outputHelper.Build(board, validLayerTypes);

        var stackLayers = BuildLayerOrder(buildResult.Layers);
        var solidLayers = BuildSolidLayers(stackLayers);

        var previewModel = new BoardPreview3DViewModel((ICanvasDesignerFileViewModel)board, _dispatcher);

        //offset
        var brdOutline = board.BoardOutline;
        if (brdOutline != null)
        {
            var bodyRect = brdOutline.GetBoundingRectangle();
            previewModel.OffsetX = -bodyRect.X - 0.5 * bodyRect.Width;
            previewModel.OffsetY = -bodyRect.Y - 0.5 * bodyRect.Height;
        }

        var models = new List<ISelectableItem>();

        var canvasModel = board.CanvasModel;

        var canvasItems = canvasModel.GetItems().ToList();

        var footprints = canvasItems.OfType<FootprintBoardCanvasItem>().ToList();

        foreach (var fp in footprints)
        {
            await LoadModelForFootprint(fp, models, boardTotalThickness, board.ProjectNode, previewModel);
        }

        previewModel.Preview3DLayers = solidLayers;
        previewModel.Items = models;

        return previewModel;
    }

    public async Task<BoardPreview3DViewModel> GeneratePreview(IFootprintDesigner footprintDesigner)
    {
        var validLayerTypes = new[] { LayerType.Signal,
                                      LayerType.Plane,
                                      LayerType.SolderMask,
                                      LayerType.SilkScreen,
                                      LayerType.Dielectric };

        var outputHelper = new BoardGlobalOutputHelper();
        var buildResult = outputHelper.Build(footprintDesigner, validLayerTypes);

        var stackLayers = BuildLayerOrder(buildResult.Layers);
        var solidLayers = BuildSolidLayers(stackLayers);

        var previewModel = new BoardPreview3DViewModel((ICanvasDesignerFileViewModel)footprintDesigner, _dispatcher);

        //offset
        var bodyRect = buildResult.BodyRectangle;
        previewModel.OffsetX = -bodyRect.X - 0.5 * bodyRect.Width;
        previewModel.OffsetY = -bodyRect.Y - 0.5 * bodyRect.Height;

        previewModel.Preview3DLayers = solidLayers;

        await Task.CompletedTask;

        return previewModel;
    }

    private IList<Preview3DLayerData> BuildSolidLayers(IList<StackLayer> stackLayers)
    {
        IList<Preview3DLayerData> solidLayers = new List<Preview3DLayerData>();


        foreach (var stackLayer in stackLayers)
        {
            var layerThickness = stackLayer.LayerThickness;
            var layerStartPos = stackLayer.LayerPositionZ;


            var meshColor = XColors.Gray;
            var layerType = stackLayer.Layer.Layer.LayerType;
            var layer = stackLayer.Layer;

            _layerTypeColors.TryGetValue(layerType, out meshColor);


            if (layerType == LayerType.SolderMask)
            {
                var previewLayer = BuildSolderMaskLayer(layer, meshColor, layerStartPos, layerThickness);
                solidLayers.Add(previewLayer);
            }
            else if (layerType == LayerType.Dielectric)
            {
                var previewLayer = BuildDielectricLayer(layer, meshColor, layerStartPos, layerThickness);
                solidLayers.Add(previewLayer);
            }
            else
            {
                var previewLayer = BuildAnyOtherLayer(layer, meshColor, layerStartPos, layerThickness);
                solidLayers.Add(previewLayer);
            }

        }

        return solidLayers;
    }



    private double GetLayerThickness(BoardGlobalLayerOutput layer)
    {
        var layerThickness = GetDefaultThicknessForLayerType(layer.Layer.LayerType);
        if (layer.Layer.Thickness > 0.01d)
            layerThickness = layer.Layer.Thickness;

        return layerThickness;
    }

    private double GetDefaultThicknessForLayerType(LayerType layerType)
    {
        var d = new Dictionary<LayerType, double>
        {
            [LayerType.Signal] = 0.01d,
            [LayerType.SolderMask] = 0.011d,
            [LayerType.Dielectric] = 1.55d,
            [LayerType.SilkScreen] = 0.01d,
        };

        if (d.ContainsKey(layerType))
            return d[layerType];

        return 0.01d;
    }

    private StackLayer GetStackLayer(BoardGlobalLayerOutput layer, double layerPosZ)
    {
        var stackLayer = new StackLayer
        {
            Layer = layer,
            LayerThickness = GetLayerThickness(layer),
        };
        layerPosZ += stackLayer.LayerThickness;
        stackLayer.LayerPositionZ = layerPosZ;

        return stackLayer;
    }

    private IList<StackLayer> BuildLayerOrder(IList<BoardGlobalLayerOutput> layers)
    {
        var stackLayers = new List<StackLayer>();

        var layerPosZ = 0.04d;

        //top silk
        var tSilk = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SilkscreenTopLayerId);
        if (tSilk != null)
        {
            var stackLayer = GetStackLayer(tSilk, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        ////top paste
        //var tPaste = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.PasteTopLayerId);
        //if (tPaste != null)
        //{
        //    var stackLayer = GetStackLayer(tPaste, layerPosZ);
        //    layerPosZ = stackLayer.LayerPositionZ;

        //    stackLayers.Add(stackLayer);
        //}

        //top mask
        var tMask = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SolderTopLayerId);
        if (tMask != null)
        {
            // var prevLayer = stackLayers.Last();
            var stackLayer = GetStackLayer(tMask, layerPosZ);
            //layerPosZ = prevLayer.LayerPositionZ;
            //stackLayer.LayerThickness += prevLayer.LayerThickness;
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        //top copper
        var tCopper = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SignalTopLayerId);
        if (tCopper != null)
        {
            var stackLayer = GetStackLayer(tCopper, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }



        //stackup inner layers in the order presented in the layers tab in properties: signal, plane, dielectric
        var bottomSideLayerIds = new[]
        {
                LayerConstants.SignalBottomLayerId,
                LayerConstants.PasteBottomLayerId,
                LayerConstants.SolderBottomLayerId,
                LayerConstants.SilkscreenBottomLayerId
            };

        var innerLayers = layers.Except(stackLayers.Select(l => l.Layer)).Where(l => !bottomSideLayerIds.Contains(l.Layer.LayerId));
        foreach (var innerLayer in innerLayers)
        {
            var stackLayer = GetStackLayer(innerLayer, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        //bottom copper
        var bCopper = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SignalBottomLayerId);
        if (bCopper != null)
        {
            var stackLayer = GetStackLayer(bCopper, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        ////bottom paste
        //var bPaste = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.PasteBottomLayerId);
        //if (bPaste != null)
        //{
        //    var stackLayer = GetStackLayer(bPaste, layerPosZ);
        //    layerPosZ = stackLayer.LayerPositionZ;

        //    stackLayers.Add(stackLayer);
        //}

        //bottom mask
        var bMask = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SolderBottomLayerId);
        if (bMask != null)
        {
            var stackLayer = GetStackLayer(bMask, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        //bottom silk
        var bSilk = layers.FirstOrDefault(l => l.Layer.LayerId == LayerConstants.SilkscreenBottomLayerId);
        if (bSilk != null)
        {
            var stackLayer = GetStackLayer(bSilk, layerPosZ);
            layerPosZ = stackLayer.LayerPositionZ;

            stackLayers.Add(stackLayer);
        }

        //we want bottom to top, so reverse
        //stackLayers.Reverse();

        return stackLayers;
    }

    private async Task LoadModelForFootprint(FootprintBoardCanvasItem fp, List<ISelectableItem> models, double boardTotalThickness,
                                 ISolutionProjectNodeModel parentProject, BoardPreview3DViewModel previewModel)
    {
        await Task.CompletedTask;
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

            // var modelDoc = await Task.Run(() => parentProject.FindObject(TemplateType.Model, modelLibrary, modelData.ModelId) as ModelDocument);
            var modelDoc = _objectFinder.FindObject<ModelDocument>(parentProject.Project, modelLibrary, modelData.ModelId);
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
                groupItem.RotationZ = sign * ( fp.Rot + modelData.RotationZ );
                if (fp.Placement == FootprintPlacement.Bottom)
                    groupItem.RotationY = ( (int)groupItem.RotationY + 180 ) % 360;

                groupItem.IsPlaced = true;

                groupItem.Items.AddRange(modelDoc.Items.Select(item => (BaseMeshItem)item.CreateDesignerItem()));
                foreach (var item in groupItem.Items)
                    item.ParentObject = groupItem;

                models.Add(groupItem);
            }
        }
    }


    private Preview3DLayerData BuildSolderMaskLayer(BoardGlobalLayerOutput layer, XColor color, double pos, double layerThickness)
    {
        var toAddItems = new List<IShape>();
        var toExcludeItems = new List<IShape>();

        toAddItems.Add(layer.BoardOutline);

        toExcludeItems.AddRange(layer.AddItems);

        var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, pos, layerThickness);

        return previewLayer;
    }

    private Preview3DLayerData BuildDielectricLayer(BoardGlobalLayerOutput layer, XColor color, double pos, double layerThickness)
    {
        var toAddItems = new List<IShape>();
        var toExcludeItems = new List<IShape>();

        toAddItems.Add(layer.BoardOutline);

        toExcludeItems.AddRange(layer.ExtractItems);

        var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, pos, layerThickness);

        return previewLayer;
    }

    private Preview3DLayerData BuildAnyOtherLayer(BoardGlobalLayerOutput layer, XColor color, double pos, double layerThickness)
    {
        var toAddItems = new List<IShape>();
        var toExcludeItems = new List<IShape>();

        //var pouredPolys = layer.AddItems.OfType<GlobalPouredPolygonPrimitive>().ToList();
        //toAddItems.AddRange(pouredPolys);
        //toAddItems.AddRange(layer.AddItems.Except(pouredPolys));
        toAddItems.AddRange(layer.AddItems);

        toExcludeItems.AddRange(layer.ExtractItems);

        var previewLayer = CreateLayer(layer, toAddItems, toExcludeItems, color, pos, layerThickness);

        return previewLayer;
    }

    private Preview3DLayerData CreateLayer(BoardGlobalLayerOutput outputLayer, IEnumerable<IShape> toAddItems, IEnumerable<IShape> toExcludeItems, XColor color, double layerStartHeight, double layerThickness)
    {
        var layer = outputLayer.Layer;
        var mesh = _meshHelper.BuildMeshFromShapes(toAddItems, toExcludeItems, layerThickness);
        return new Preview3DLayerData
        {
            Mesh = mesh,
            Color = color,
            DisplayColor = color,
            LayerColor = layer.LayerColor,
            LayerType = layer.LayerType,
            PositionZ = layerStartHeight
        };
    }

    class StackLayer
    {
        public BoardGlobalLayerOutput Layer { get; set; }

        public double LayerPositionZ { get; set; }

        public double LayerThickness { get; set; }
    }
}
