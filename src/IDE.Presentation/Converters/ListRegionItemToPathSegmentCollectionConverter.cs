using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace IDE.Core.Converters
{
    public class ListRegionItemToPathSegmentCollectionConverter : IValueConverter
    {
        static ListRegionItemToPathSegmentCollectionConverter _instance;
        public static ListRegionItemToPathSegmentCollectionConverter Instance
        {
            get { return _instance ?? (_instance = new ListRegionItemToPathSegmentCollectionConverter()); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = value as IList<IRegionItem>;
            if (items == null)
                return null;

            var segmentCollection = new PathSegmentCollection();

            foreach (var item in items)
            {
                PathSegment segment = null;
                var epDPI = MilimetersToDpiHelper.ConvertToDpi(item.EndPoint).ToPoint();

                if (item is LineRegionItem)
                {
                    var lineItem = item as LineRegionItem;

                    segment = new LineSegment(epDPI, true)
                    {
                        IsSmoothJoin = true
                    };

                }
                else if (item is ArcRegionItem)
                {
                    var arcItem = item as ArcRegionItem;
                    var diameterDPI = MilimetersToDpiHelper.ConvertToDpi(arcItem.SizeDiameter);

                    segment = new ArcSegment
                    {
                        Point = epDPI,
                        IsStroked = true,
                        IsSmoothJoin = true,
                        IsLargeArc = arcItem.IsLargeArc,
                        SweepDirection = arcItem.SweepDirection.ToSweepDirection(),
                        Size = new Size(diameterDPI, diameterDPI)
                    };
                }

                segmentCollection.Add(segment);
            }

            return segmentCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
