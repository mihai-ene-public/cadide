using IDE.Core.Interfaces;
using IDE.Core.Resources;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Core.Converters
{
    public class TemplateTypeToImageConverter : IValueConverter
    {
        static TemplateTypeToImageConverter instance;
        public static TemplateTypeToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateTypeToImageConverter();

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
            if (value is TemplateType)
            {
                var tt = (TemplateType)value;
                switch (tt)
                {
                    case TemplateType.Project:
                        return GetDrawingImage("filetype.project");

                    case TemplateType.Component:
                        return GetDrawingImage("filetype.component");

                    case TemplateType.Footprint:
                        return GetDrawingImage("filetype.footprint");

                    case TemplateType.Model:
                        return GetDrawingImage("filetype.model");

                    case TemplateType.Symbol:
                        return GetDrawingImage("filetype.symbol");

                    case TemplateType.Schematic:
                        return GetDrawingImage("filetype.schematic");

                    case TemplateType.Board:
                        return GetDrawingImage("filetype.board");

                }
            }

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
