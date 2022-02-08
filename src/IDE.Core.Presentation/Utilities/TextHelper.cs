using IDE.Core.Converters;
using IDE.Core.Types.Media;
using SixLabors.Fonts;

namespace IDE.Core.Presentation.Utilities;

public static class TextHelper
{
    public static XRect MeasureString(string text, string fontName, double fontSize, bool bold = false, bool italic = false, bool underline = false, double wrapWidth = -1.0)
    {
        var font = SixLabors.Fonts.SystemFonts.CreateFont(fontName, (float)fontSize);
        if (!double.IsNaN(wrapWidth) && wrapWidth > 0.0)
        {
            wrapWidth = MilimetersToDpiHelper.ConvertToDpi(wrapWidth);
        }
        var size = TextMeasurer.Measure(text, new RendererOptions(font, 96.0f) { WrappingWidth = (float)wrapWidth });

        var rect = new XRect(0, 0, size.Width, size.Height);

        rect = MilimetersToDpiHelper.ConvertToMM(rect);

        return rect;
    }
}
