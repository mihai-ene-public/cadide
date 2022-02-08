using IDE.Core.Types.Media;
using System.Windows;

namespace IDE.Core.Converters
{
    public class MilimetersToDpiHelper
    {
        //we target device independent
        //it should depend on monitor
        //96 dots per inch =  96 dots in 25.4 mm
        const double DotsPerMM = 96.0 / 25.4;

        // public double Scale { get; set; }

        static double GetMonitorDPI
        {
            get
            {
                //var v = new System.Windows.Media.RectangleGeometry();
                return 1;
            }
        }

        public static double MillimetersToDpiTransformFactor => GetMonitorDPI * DotsPerMM;

        public static double ConvertToDpi(double mmValue)
        {
            var dots = GetMonitorDPI * DotsPerMM * mmValue;
            return dots;
        }

        public static XRect ConvertToDpi(XRect mmRect)
        {
            var dotsRect = new XRect(ConvertToDpi(mmRect.X)
                                  , ConvertToDpi(mmRect.Y)
                                  , ConvertToDpi(mmRect.Width)
                                  , ConvertToDpi(mmRect.Height));

            return dotsRect;
        }

        public static XPoint ConvertToDpi(XPoint mmPoint)
        {
            var dotsPoint = new XPoint(ConvertToDpi(mmPoint.X)
                                    , ConvertToDpi(mmPoint.Y)
                                  );

            return dotsPoint;
        }

        public static double ConvertToMM(double dpiValue)
        {
            var mm = dpiValue / DotsPerMM;
            return mm;
        }

        public static XRect ConvertToMM(XRect dpiRect)
        {
            var mmRect = new XRect(ConvertToMM(dpiRect.X)
                                 , ConvertToMM(dpiRect.Y)
                                 , ConvertToMM(dpiRect.Width)
                                 , ConvertToMM(dpiRect.Height));

            return mmRect;
        }

        public static XPoint ConvertToMM(XPoint dpiPoint)
        {
            return new XPoint(ConvertToMM(dpiPoint.X)
                            , ConvertToMM(dpiPoint.Y));
        }
    }
}
