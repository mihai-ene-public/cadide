using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IDE.Core.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        static BytesToImageConverter instance;
        public static BytesToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new BytesToImageConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = value as byte[];
            var image = new BitmapImage();
            try
            {
                if (bytes != null)
                {
                    using (var byteStream = new MemoryStream(bytes))
                    {
                       
                        image.BeginInit();
                        image.UriSource = null;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = byteStream;
                        image.EndInit();

                        return image;
                    }
                }
            }
            catch { }

            return DependencyProperty.UnsetValue;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
