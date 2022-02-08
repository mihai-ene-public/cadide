using IDE.Core.Designers;
using IDE.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Core.Converters
{
    public class ToolboxPrimitiveToImageConverter : IValueConverter
    {

        static ToolboxPrimitiveToImageConverter()
        {
            Instance = new ToolboxPrimitiveToImageConverter();
        }


        public static ToolboxPrimitiveToImageConverter Instance { get; private set; }

        DrawingImage GetDrawingImage(string name)
        {
            return ResourceLocator.GetResource<DrawingImage>(
                                      "IDE.Presentation",
                                      "_Themes/MetroIcons/Icons.xaml",
                                      name) as DrawingImage;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as Type;
            if (type == null)
                return Binding.DoNothing;

            try
            {
                if (type == typeof(LineSchematicCanvasItem) || type == typeof(LineBoardCanvasItem))
                    return GetDrawingImage("toolbox.line");
                if (type == typeof(TextCanvasItem) || type == typeof(TextBoardCanvasItem))// || type == typeof(TextSingleLineBoardCanvasItem))
                    return GetDrawingImage("toolbox.text");
                if (type == typeof(TextSingleLineBoardCanvasItem))
                    return GetDrawingImage("toolbox.TextMonoline");
                if (type == typeof(RectangleCanvasItem) || type == typeof(RectangleBoardCanvasItem))
                    return GetDrawingImage("toolbox.rectangle");
                if (type == typeof(ImageCanvasItem))
                    return GetDrawingImage("toolbox.image");
                if (type == typeof(PolygonCanvasItem) || type == typeof(PolygonBoardCanvasItem))
                    return GetDrawingImage("toolbox.polygon");
                if (type == typeof(CircleCanvasItem) || type == typeof(CircleBoardCanvasItem))
                    return GetDrawingImage("toolbox.circle");
                if (type == typeof(EllipseCanvasItem))// || type == typeof(EllipseBoardCanvasItem))
                    return GetDrawingImage("toolbox.ellipse");
                if (type == typeof(ArcCanvasItem) || type == typeof(ArcBoardCanvasItem))
                    return GetDrawingImage("toolbox.arc");
                if (type == typeof(PinCanvasItem))
                    return GetDrawingImage("toolbox.pin");
                if (type == typeof(NetWireCanvasItem))
                    return GetDrawingImage("toolbox.netWire");
                if (type == typeof(JunctionCanvasItem))
                    return GetDrawingImage("toolbox.junction");
                if (type == typeof(NetLabelCanvasItem))
                    return GetDrawingImage("toolbox.netLabel");
                if (type == typeof(BusWireCanvasItem))
                    return GetDrawingImage("toolbox.busWire");
                if (type == typeof(BusLabelCanvasItem))
                    return GetDrawingImage("toolbox.busLabel");

                if (type == typeof(HoleCanvasItem))
                    return GetDrawingImage("toolbox.hole");
                if (type == typeof(PadThtCanvasItem))
                    return GetDrawingImage("toolbox.pad");
                if (type == typeof(PadSmdCanvasItem))
                    return GetDrawingImage("toolbox.smd");
                if (type == typeof(TrackBoardCanvasItem))
                    return GetDrawingImage("toolbox.trace");
                if (type == typeof(ViaCanvasItem))
                    return GetDrawingImage("toolbox.via");
                if (type == typeof(SchematicSymbolCanvasItem) || type==typeof(FootprintBoardCanvasItem))
                    return GetDrawingImage("toolbox.addPart");
                
                if (type == typeof(BoxMeshItem))
                    return GetDrawingImage("toolbox.box3d");
                if (type == typeof(ConeMeshItem))
                    return GetDrawingImage("toolbox.cone");
                if (type == typeof(CylinderMeshItem))
                    return GetDrawingImage("toolbox.cylinder");
                if (type == typeof(SphereMeshItem))
                    return GetDrawingImage("toolbox.sphere");
                if (type == typeof(EllipsoidMeshItem))
                    return GetDrawingImage("toolbox.ellipsoid");
                if (type == typeof(ExtrudedPolyMeshItem))
                    return GetDrawingImage("toolbox.extrudedpolygon");
                if (type == typeof(TextMeshItem))
                    return GetDrawingImage("toolbox.text");

            }
            catch
            {

            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
