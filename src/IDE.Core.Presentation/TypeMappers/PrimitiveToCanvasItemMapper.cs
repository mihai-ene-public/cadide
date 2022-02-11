using IDE.Core.Common.Utilities;
using IDE.Core.Interfaces;
using System;
using IDE.Core.Storage;
using IDE.Core.Designers;

namespace IDE.Core.ViewModels
{
    public class PrimitiveToCanvasItemMapper : GenericMapper, IPrimitiveToCanvasItemMapper
    {
        public PrimitiveToCanvasItemMapper() : base()
        {

        }

        protected override void CreateMappings()
        {
            //board
            AddMapping(typeof(ArcBoard), typeof(ArcBoardCanvasItem));
            AddMapping(typeof(CircleBoard), typeof(CircleBoardCanvasItem));
            AddMapping(typeof(BoardComponentInstance), typeof(FootprintBoardCanvasItem));
            AddMapping(typeof(Hole), typeof(HoleCanvasItem));
            AddMapping(typeof(LineBoard), typeof(LineBoardCanvasItem));
            //AddMapping(typeof(PadRef), typeof(PadRefDesignerItem));
            AddMapping(typeof(PolygonBoard), typeof(PolygonBoardCanvasItem));
            AddMapping(typeof(RectangleBoard), typeof(RectangleBoardCanvasItem));
            AddMapping(typeof(TextBoard), typeof(TextBoardCanvasItem));
            AddMapping(typeof(TextSingleLineBoard), typeof(TextSingleLineBoardCanvasItem));
            //AddMapping(typeof(TraceBoard), typeof(TraceBoardCanvasItem));
            AddMapping(typeof(TrackBoard), typeof(TrackBoardCanvasItem));
            AddMapping(typeof(PlaneBoard), typeof(PlaneBoardCanvasItem));
            AddMapping(typeof(Via), typeof(ViaCanvasItem));
            AddMapping(typeof(Pad), typeof(PadThtCanvasItem));
            AddMapping(typeof(Smd), typeof(PadSmdCanvasItem));

            //schematic
            AddMapping(typeof(Pin), typeof(PinCanvasItem));
            AddMapping(typeof(Arc), typeof(ArcCanvasItem));
            AddMapping(typeof(Circle), typeof(CircleCanvasItem));
            AddMapping(typeof(Ellipse), typeof(EllipseCanvasItem));
            AddMapping(typeof(ImagePrimitive), typeof(ImageCanvasItem));
            AddMapping(typeof(Instance), typeof(SchematicSymbolCanvasItem));
            AddMapping(typeof(Junction), typeof(JunctionCanvasItem));
            AddMapping(typeof(LineSchematic), typeof(LineSchematicCanvasItem));
            AddMapping(typeof(NetLabel), typeof(NetLabelCanvasItem));
            AddMapping(typeof(NetWire), typeof(NetWireCanvasItem));
            AddMapping(typeof(BusWire), typeof(BusWireCanvasItem));
            AddMapping(typeof(BusLabel), typeof(BusLabelCanvasItem));
            //AddMapping(typeof(PinRef), typeof(PinRefDesignerItem));
            AddMapping(typeof(Polygon), typeof(PolygonCanvasItem));
            AddMapping(typeof(Rectangle), typeof(RectangleCanvasItem));
            AddMapping(typeof(Text), typeof(TextCanvasItem));

            //mesh items
            AddMapping(typeof(Cylinder3DItem), typeof(CylinderMeshItem));
            AddMapping(typeof(Mesh3DItem), typeof(SolidBodyMeshItem));
            AddMapping(typeof(Text3DItem), typeof(TextMeshItem));
            AddMapping(typeof(Cone3DItem), typeof(ConeMeshItem));
            AddMapping(typeof(ExtrudedPoly3DItem), typeof(ExtrudedPolyMeshItem));
            AddMapping(typeof(Group3DItem), typeof(GroupMeshItem));
            AddMapping(typeof(Box3DItem), typeof(BoxMeshItem));
            AddMapping(typeof(Ellipsoid3DItem), typeof(EllipsoidMeshItem));
            AddMapping(typeof(Sphere3DItem), typeof(SphereMeshItem));

            AddMapping(typeof(AxialParametricPackage3DItem), typeof(AxialParametricPackageMeshItem));
            AddMapping(typeof(BGAParametricPackage3DItem), typeof(BGAParametricPackageMeshItem));
            AddMapping(typeof(ChipParametricPackage3DItem), typeof(ChipParametricPackageMeshItem));
            AddMapping(typeof(CrystalSMDParametricPackage3DItem), typeof(CrystalSMDParametricPackageMeshItem));
            AddMapping(typeof(DIPParametricPackage3DItem), typeof(DIPParametricPackageMeshItem));
            AddMapping(typeof(DFNParametricPackage3DItem), typeof(DFNParametricPackageMeshItem));
            AddMapping(typeof(DPakParametricPackage3DItem), typeof(DPakParametricPackageMeshItem));
            AddMapping(typeof(ECapParametricPackage3DItem), typeof(ECapParametricPackageMeshItem));
            AddMapping(typeof(MelfParametricPackage3DItem), typeof(MelfParametricPackageMeshItem));
            AddMapping(typeof(SODParametricPackage3DItem), typeof(SODParametricPackageMeshItem));
            AddMapping(typeof(SMAParametricPackage3DItem), typeof(SMAParametricPackageMeshItem));
            AddMapping(typeof(SOICParametricPackage3DItem), typeof(SOICParametricPackageMeshItem));
            AddMapping(typeof(SOT23ParametricPackage3DItem), typeof(SOT23ParametricPackageMeshItem));
            AddMapping(typeof(SOT223ParametricPackage3DItem), typeof(SOT223ParametricPackageMeshItem));
            AddMapping(typeof(QFNParametricPackage3DItem), typeof(QFNParametricPackageMeshItem));
            AddMapping(typeof(QFPParametricPackage3DItem), typeof(QFPParametricPackageMeshItem));
            AddMapping(typeof(PinHeaderStraightParametricPackage3DItem), typeof(PinHeaderStraightParametricPackageMeshItem));
            AddMapping(typeof(RadialGenericRoundParametricPackage3DItem), typeof(RadialGenericRoundParametricPackageMeshItem));
            AddMapping(typeof(RadialLEDParametricPackage3DItem), typeof(RadialLEDParametricPackageMeshItem));
        }

        public ISelectableItem CreateDesignerItem(IPrimitive primitive)
        {
            var mappedType = GetMapping(primitive.GetType());
            if (mappedType != null)
            {
                var canvasItem = Activator.CreateInstance(mappedType) as ISelectableItem;

                if (canvasItem != null)
                {
                    canvasItem.LoadFromPrimitive(primitive);
                    return canvasItem;
                }
            }

            throw new NotSupportedException();
        }
    }
}
