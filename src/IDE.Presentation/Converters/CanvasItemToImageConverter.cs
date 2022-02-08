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
   public class CanvasItemToImageConverter : IValueConverter
    {
        static CanvasItemToImageConverter instance;

        public static CanvasItemToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new CanvasItemToImageConverter();

                return instance;
            }
        }

        Dictionary<string, DrawingImage> cachedImages = new Dictionary<string, DrawingImage>();

        DrawingImage GetDrawingImage(string name)
        {
            if (cachedImages.ContainsKey(name))
                return cachedImages[name];



            var image = ResourceLocator.GetResource<DrawingImage>(
                                      "IDE.Presentation",
                                      "_Themes/MetroIcons/Icons.xaml",
                                      name) as DrawingImage;
            cachedImages.Add(name, image);

            return image;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var type = value as Type;
            if (value == null)
                return GetDefault();

            try
            {
                if (value is LineSchematicCanvasItem || value is LineBoardCanvasItem)
                    return GetDrawingImage("toolbox.line");
                if (value is TextCanvasItem || value is TextBoardCanvasItem)// || value is TextSingleLineBoardCanvasItem))
                    return GetDrawingImage("toolbox.text");
                if (value is TextSingleLineBoardCanvasItem)
                    return GetDrawingImage("toolbox.TextMonoline");
                if (value is RectangleCanvasItem || value is RectangleBoardCanvasItem)
                    return GetDrawingImage("toolbox.rectangle");
                if (value is ImageCanvasItem)
                    return GetDrawingImage("toolbox.image");
                if (value is PolygonCanvasItem || value is PolygonBoardCanvasItem)
                    return GetDrawingImage("toolbox.polygon");
                if (value is CircleCanvasItem || value is CircleBoardCanvasItem)
                    return GetDrawingImage("toolbox.circle");
                if (value is EllipseCanvasItem)// || value is EllipseBoardCanvasItem))
                    return GetDrawingImage("toolbox.ellipse");
                if (value is ArcCanvasItem || value is ArcBoardCanvasItem)
                    return GetDrawingImage("toolbox.arc");
                if (value is PinCanvasItem)
                    return GetDrawingImage("toolbox.pin");
                if (value is NetWireCanvasItem)
                    return GetDrawingImage("toolbox.netWire");
                if (value is JunctionCanvasItem)
                    return GetDrawingImage("toolbox.junction");
                if (value is NetLabelCanvasItem)
                    return GetDrawingImage("toolbox.netLabel");
                if(value is BusWireCanvasItem)
                    return GetDrawingImage("toolbox.busWire");
                if (value is BusLabelCanvasItem)
                    return GetDrawingImage("toolbox.busLabel");

                if (value is HoleCanvasItem)
                    return GetDrawingImage("toolbox.hole");
                if (value is PadThtCanvasItem)
                    return GetDrawingImage("toolbox.pad");
                if (value is PadSmdCanvasItem)
                    return GetDrawingImage("toolbox.smd");
                if (value is TrackBoardCanvasItem)
                    return GetDrawingImage("toolbox.trace");
                if (value is ViaCanvasItem)
                    return GetDrawingImage("toolbox.via");
                if (value is SchematicSymbolCanvasItem || value is FootprintBoardCanvasItem)
                    return GetDrawingImage("toolbox.addPart");
                if (value is BoxMeshItem)
                    return GetDrawingImage("toolbox.box3d");
                if (value is ConeMeshItem)
                    return GetDrawingImage("toolbox.cone");
                if (value is CylinderMeshItem)
                    return GetDrawingImage("toolbox.cylinder");
                if (value is SphereMeshItem)
                    return GetDrawingImage("toolbox.sphere");
                if (value is EllipsoidMeshItem)
                    return GetDrawingImage("toolbox.ellipsoid");
                if (value is ExtrudedPolyMeshItem)
                    return GetDrawingImage("toolbox.extrudedpolygon");
                if (value is TextMeshItem)
                    return GetDrawingImage("toolbox.text");

                //default
                return GetDefault();

            }
            catch
            {

            }

            return Binding.DoNothing;
        }

        BitmapImage GetDefault()
        {
            var icon = new BitmapImage();
            try
            {
                var resourceUri = "Images/document.png";
                icon.BeginInit();
                icon.UriSource = new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/{1}", "IDE.Resources", resourceUri));
                icon.EndInit();
            }
            catch
            {
                return null;
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
