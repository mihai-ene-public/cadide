using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Placement;

public class PlacementToolFactory : IPlacementToolFactory
{
    public PlacementToolFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly IServiceProvider _serviceProvider;

    public IPlacementTool Create(Type canvasItemType)
    {
        var mapping = GetMapping();

        if (mapping.ContainsKey(canvasItemType))
        {
            var placementToolType = mapping[canvasItemType];
            var placementTool = _serviceProvider.GetService(placementToolType) as PlacementTool;
            return placementTool;
        }

        return null;
    }

    public IPlacementTool Create(IToolboxItem toolboxItem)
    {
        var placementToolType = toolboxItem.PlacementToolType;
        if (placementToolType != null)
        {
            var placementTool = (IPlacementTool)Activator.CreateInstance(placementToolType);
            return placementTool;
        }

        var pt = Create(toolboxItem.Type);
        if (pt != null)
            return pt;

        throw new NotSupportedException();
    }

    private Dictionary<Type, Type> GetMapping()
    {
        var mapping = new Dictionary<Type, Type>()
        {
            //sphere
             { typeof(SphereMeshItem),typeof(ISphereMeshItemPlacementTool)},

            //box
             { typeof(BoxMeshItem),typeof(IBoxMeshItemPlacementTool)},

             //text 3d
             { typeof(TextMeshItem),typeof(ITextMeshItemPlacementTool)},

            //cone
             { typeof(ConeMeshItem),typeof(IConeMeshItemPlacementTool)},

            //cylinder
             { typeof(CylinderMeshItem),typeof(ICylinderMeshItemPlacementTool)},

            //ellipsoid
             { typeof(EllipsoidMeshItem),typeof(IEllipsoidMeshItemPlacementTool)},

            //extruded poly
             { typeof(ExtrudedPolyMeshItem),typeof(IExtrudedPolyMeshItemPlacementTool)},


            //group
            { typeof(VolatileGroup3DCanvasItem),typeof(IVolatileGroup3DPlacementTool)},
            { typeof(VolatileGroupCanvasItem),typeof(IVolatileGroupPlacementTool)},

            //line
            { typeof(LineSchematicCanvasItem),typeof(ILinePlacementTool)},
            { typeof(LineBoardCanvasItem),typeof(ILinePlacementTool)},

            //arc
            { typeof(ArcCanvasItem),typeof(IArcPlacementTool)},
            { typeof(ArcBoardCanvasItem),typeof(IArcPlacementTool)},

            //text
            { typeof(TextCanvasItem),typeof(ITextPlacementTool)},
            { typeof(TextBoardCanvasItem),typeof(ITextPlacementTool)},
            { typeof(TextSingleLineBoardCanvasItem),typeof(ITextPlacementTool)},

            //rect
            { typeof(RectangleCanvasItem),typeof(IRectanglePlacementTool)},
            { typeof(RectangleBoardCanvasItem),typeof(IRectanglePlacementTool)},

            //poly
            { typeof(PolygonCanvasItem),typeof(IPolygonPlacementTool)},
            { typeof(PolygonBoardCanvasItem),typeof(IPolygonPlacementTool)},

            //circle
            { typeof(CircleCanvasItem),typeof(ICirclePlacementTool)},
            { typeof(CircleBoardCanvasItem),typeof(ICirclePlacementTool)},

            //ellipse
            { typeof(EllipseCanvasItem),typeof(IEllipsePlacementTool)},

            //image
            { typeof(ImageCanvasItem),typeof(IImagePlacementTool)},

            //pin
            { typeof(PinCanvasItem),typeof(IPinPlacementTool)},

            //hole
            { typeof(HoleCanvasItem),typeof(IHolePlacementTool)},

            //pad/smd
            { typeof(PadThtCanvasItem),typeof(IPadPlacementTool)},
            { typeof(PadSmdCanvasItem),typeof(IPadPlacementTool)},

            //sch net wire
            { typeof(NetWireCanvasItem),typeof(INetWirePlacementTool)},

            //sch bus wire
            { typeof(BusWireCanvasItem),typeof(IBusWirePlacementTool)},

            //junction
            { typeof(JunctionCanvasItem),typeof(IJunctionPlacementTool)},

            //net label
            { typeof(NetLabelCanvasItem),typeof(INetLabelPlacementTool)},

            //bus label
            { typeof(BusLabelCanvasItem),typeof(IBusLabelPlacementTool)},

            //part
            { typeof(SchematicSymbolCanvasItem),typeof(IPartPlacementTool)},

            //board via
            { typeof(ViaCanvasItem),typeof(IViaPlacementTool)},


        };
        return mapping;
    }

}
