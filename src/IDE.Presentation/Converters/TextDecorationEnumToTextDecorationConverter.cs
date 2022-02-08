using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using IDE.Core.Storage;

namespace IDE.Core.Converters
{
    public class TextDecorationEnumToTextDecorationConverter : IValueConverter
    {

        static TextDecorationEnumToTextDecorationConverter instance;
        public static TextDecorationEnumToTextDecorationConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextDecorationEnumToTextDecorationConverter();

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextDecorationEnum)
            {
                var td = (TextDecorationEnum)value;
                switch (td)
                {
                    case TextDecorationEnum.None:
                        return null;
                    case TextDecorationEnum.Baseline:
                        return TextDecorations.Baseline;
                    case TextDecorationEnum.OverLine:
                        return TextDecorations.OverLine;
                    case TextDecorationEnum.Strikethrough:
                        return TextDecorations.Strikethrough;
                    case TextDecorationEnum.Underline:
                        return TextDecorations.Underline;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
