using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Core.Converters
{
    [ValueConversion(typeof(pinOrientation), typeof(double))]
    public class PinOrientationToAngleConverter : IValueConverter
    {
        static PinOrientationToAngleConverter()
        {
            Instance = new PinOrientationToAngleConverter();
        }

        public static PinOrientationToAngleConverter Instance
        {
            get;
            private set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pinOrientation = (pinOrientation)value;
            var paramString = parameter as string;

            //we make the text to be always displayed as to be easy to be read either from top-bottom or on the side
            if (paramString == "text")
            {
                switch (pinOrientation)
                {
                    case pinOrientation.Right:
                    case pinOrientation.Left:
                        //should end zero
                        return -(double)pinOrientation;
                    case pinOrientation.Up:
                    case pinOrientation.Down:
                        //should end up 90
                        return 270d - (double)pinOrientation;
                }


            }
            else
            {
                //clockwise
                return (double)pinOrientation;
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
