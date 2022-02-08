namespace IDE.Core.Converters
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Data;

    public class CountToVisibilityConverter : IValueConverter
    {
        static CountToVisibilityConverter instance;
        public static CountToVisibilityConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new CountToVisibilityConverter();

                return instance;
            }
        }

       
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IList list && list.Count > 0)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
