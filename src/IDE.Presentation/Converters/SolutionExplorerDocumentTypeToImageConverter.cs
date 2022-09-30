using FontAwesome5;
using IDE.Core.Interfaces;
using IDE.Core.Resources;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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

        Dictionary<string, ImageSource> cachedImages = new Dictionary<string, ImageSource>();

        private ImageSource GetDrawingImage(string name)
        {
            if (cachedImages.ContainsKey(name))
                return cachedImages[name];

            var image = ResourceLocator.GetResource<DrawingImage>(
                                      "IDE.Presentation",
                                      "_Themes/MetroIcons/Icons.xaml",
                                      name);
            cachedImages.Add(name, image);

            return image;
        }
        private ImageSource GetImageSource(EFontAwesomeIcon icon, Brush brush)
        {
            var name = icon.ToString();
            if (cachedImages.ContainsKey(name))
                return cachedImages[name];

            var image = ImageAwesome.CreateImageSource(icon, brush);
            cachedImages.Add(name, image);

            return image;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

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
            }
            else if (value is ProjectBoardNodeModel)
            {
                return GetDrawingImage("filetype.board");
            }
            else if (value is ProjectFolderNodeModel)
            {
                return GetDrawingImage("filetype.folder");
            }
            else if (value is ProjectSymbolNodeModel)
            {
                return GetDrawingImage("filetype.symbol");
            }
            else if (value is ProjectFootprintNodeModel)
            {
                return GetDrawingImage("filetype.footprint");
            }
            else if (value is ProjectComponentNodeModel)
            {
                return GetDrawingImage("filetype.component");
            }
            else if (value is ProjectModelNodeModel)
            {
                return GetDrawingImage("filetype.model");
            }
            else if (value is ISolutionExplorerNodeModel slnNodeModel)
            {
                var fileExtension = Path.GetExtension(slnNodeModel.FileName);
                switch (fileExtension)
                {
                    case ".txt":
                        return GetImageSource(EFontAwesomeIcon.Regular_FileAlt, Brushes.White);

                    case ".pdf":
                        return GetImageSource(EFontAwesomeIcon.Regular_FilePdf, Brushes.Brown);
                }
            }

            return GetImageSource(EFontAwesomeIcon.Regular_Circle, Brushes.White);
            //return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
