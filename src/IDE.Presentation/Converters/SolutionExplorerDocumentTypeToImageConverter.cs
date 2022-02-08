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
    public class SolutionExplorerDocumentTypeToImageConverter : IValueConverter
    {

        static SolutionExplorerDocumentTypeToImageConverter instance;

        public static SolutionExplorerDocumentTypeToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new SolutionExplorerDocumentTypeToImageConverter();

                return instance;
            }
        }

        Dictionary<string, DrawingImage> cachedImages = new Dictionary<string, DrawingImage>();

        DrawingImage GetDrawingImage(string name)
        {
            if (cachedImages.ContainsKey(name))
                return cachedImages[name];



            var image= ResourceLocator.GetResource<DrawingImage>(
                                      "IDE.Presentation",
                                      "_Themes/MetroIcons/Icons.xaml",
                                      name) as DrawingImage;
            cachedImages.Add(name, image);

            return image;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //return Binding.DoNothing;

            if (value is SolutionRootNodeModel)
            {
                return GetDrawingImage("filetype.solution");
            }
            else if (value is SolutionProjectNodeModel)
            {
                return GetDrawingImage("filetype.project");
            }
            else if (value is ProjectSchematicNodeModel)
            {
                return GetDrawingImage("filetype.schematic");
                //resourceUri = "Images/documents/schematic.png";
            }
            else if (value is ProjectBoardNodeModel)
            {
                return GetDrawingImage("filetype.board");
                //resourceUri = "Images/documents/pcb.png";
            }
            else if (value is ProjectFolderNodeModel)
            {
                return GetDrawingImage("filetype.folder");
                //resourceUri = "Images/documents/folder.png";
            }
            else if (value is ProjectSymbolNodeModel)
            {
                return GetDrawingImage("filetype.symbol");
                //resourceUri = "Images/documents/symbol.png";
            }
            else if (value is ProjectFootprintNodeModel)
            {
                return GetDrawingImage("filetype.footprint");
                //resourceUri = "Images/documents/footprint.png";
            }
            else if (value is ProjectComponentNodeModel)
            {
                return GetDrawingImage("filetype.component");
                // resourceUri = "Images/documents/component.png";
            }
            else if (value is ProjectModelNodeModel)
                return GetDrawingImage("filetype.model");

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
