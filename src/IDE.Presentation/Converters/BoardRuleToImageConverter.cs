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
    public class BoardRuleToImageConverter : IValueConverter
    {
        static BoardRuleToImageConverter instance;
        public static BoardRuleToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new BoardRuleToImageConverter();

                return instance;
            }
        }

        DrawingImage GetDrawingImage(string name)
        {
            return ResourceLocator.GetResource<DrawingImage>(
                                      "IDE.Presentation",
                                      "_Themes/MetroIcons/Icons.xaml",
                                      name) as DrawingImage;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GroupRuleModel)
                return GetDrawingImage("folder-open");
            else if (value is ElectricalClearanceRuleModel)
                return GetDrawingImage("rules.electricalClearance");
            else if (value is TrackWidthRuleModel)
                return GetDrawingImage("rules.trackWidth");
            else if (value is ViaDefinitionRuleModel)
                return GetDrawingImage("rules.viaDefinition");
            else if (value is ManufacturingHoleSizeRuleModel)
                return GetDrawingImage("rules.holeSize");
            else if (value is MaskExpansionRuleModel)
                return GetDrawingImage("rules.maskExpansion");
            else if (value is ManufacturingClearanceRuleModel)
                return GetDrawingImage("rules.manufClearance");

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
                return Binding.DoNothing;
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
