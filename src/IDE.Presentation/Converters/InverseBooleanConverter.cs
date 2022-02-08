namespace IDE.Core.Converters
{
    using System;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {

        static InverseBooleanConverter instance;
        public static InverseBooleanConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new InverseBooleanConverter();
                return instance;
            }
        }


        public object Convert(object value, Type targetType, object parameter,
                                                    System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                                            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

    }
}
