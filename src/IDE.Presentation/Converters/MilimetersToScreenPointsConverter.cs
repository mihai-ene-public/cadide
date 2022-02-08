using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IDE.Core.Converters
{

    /// <summary>
    /// at a specific scale, the provided value in mm will be the same value on the monitor screen in mm
    /// </summary>
    public class MilimetersToScreenPointsDoubleConverter : IValueConverter
    {
        public MilimetersToScreenPointsDoubleConverter()
        {
        }

        static MilimetersToScreenPointsDoubleConverter instance;

        public static MilimetersToScreenPointsDoubleConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new MilimetersToScreenPointsDoubleConverter();

                return instance;
            }
        }




        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mm = (double)value;
            var dots = MilimetersToDpiHelper.ConvertToDpi(mm);
            return dots;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MilimetersToScreenPointsRectConverter : IValueConverter
    {
        public MilimetersToScreenPointsRectConverter()
        {
        }

        static MilimetersToScreenPointsRectConverter instance;

        public static MilimetersToScreenPointsRectConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new MilimetersToScreenPointsRectConverter();

                return instance;
            }
        }




        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mmRect = value is XRect ? (XRect)value : ((Rect)value).ToXRect();

            var dotsRect = new Rect(MilimetersToDpiHelper.ConvertToDpi(mmRect.X)
                                  , MilimetersToDpiHelper.ConvertToDpi(mmRect.Y)
                                  , MilimetersToDpiHelper.ConvertToDpi(mmRect.Width)
                                  , MilimetersToDpiHelper.ConvertToDpi(mmRect.Height));
            return dotsRect;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MilimetersToScreenPointConverter : IValueConverter
    {
        public MilimetersToScreenPointConverter()
        {
            Scale = 1;
        }

        static MilimetersToScreenPointConverter instance;

        public static MilimetersToScreenPointConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new MilimetersToScreenPointConverter();

                return instance;
            }
        }


        public double Scale { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mmPoint = (XPoint)value;
            var dotsRect = new Point(MilimetersToDpiHelper.ConvertToDpi(mmPoint.X)
                                  , MilimetersToDpiHelper.ConvertToDpi(mmPoint.Y)
                                  );
            return dotsRect;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MilimetersToScreenSizeConverter : IValueConverter
    {
        public MilimetersToScreenSizeConverter()
        {
        }

        static MilimetersToScreenSizeConverter instance;

        public static MilimetersToScreenSizeConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new MilimetersToScreenSizeConverter();

                return instance;
            }
        }




        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mmPoint = (XSize)value;
            var dotsRect = new Size(MilimetersToDpiHelper.ConvertToDpi(mmPoint.Width)
                                  , MilimetersToDpiHelper.ConvertToDpi(mmPoint.Height)
                                  );
            return dotsRect;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
