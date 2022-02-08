using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace IDE.Core.Converters
{

    //we want to have the width of something with text arranged on a standard grid (should not be the current grid)
    //for a pin (in particular)
    //this will align the pin line regardless of the orientation of the pin to be on the standard grid
    public class TextToGridWidthConverter : IValueConverter
    {

        static TextToGridWidthConverter()
        {
            Instance = new TextToGridWidthConverter();
        }


        public static TextToGridWidthConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const double standardGridSize = 10.0;
            const double fontSize = 16;


            if(value==null)
                return Binding.DoNothing;

            var pString = value.ToString();
            {
                //this will be dependent on the font used by the text renderer itself; for now hardcoded values should do
                var textWidth = GetWidth(pString, fontSize);

                //a whole number of grid size
                var gridUnits = (int)(Math.Round(textWidth / standardGridSize + 0.5));
                var width = gridUnits * standardGridSize + 5;

                return width;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        double GetWidth(string text, double fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var typeface = new Typeface(new FontFamily("Segoe UI"),
                                FontStyles.Normal,
                                FontWeights.Normal,
                                FontStretches.Normal);

            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No glyphtypeface found");


            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];

            double totalWidth = 0;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                glyphIndexes[n] = glyphIndex;

                double width = glyphTypeface.AdvanceWidths[glyphIndex] * fontSize;
                advanceWidths[n] = width;

                totalWidth += width;
            }

            return totalWidth;
        }
    }
}
