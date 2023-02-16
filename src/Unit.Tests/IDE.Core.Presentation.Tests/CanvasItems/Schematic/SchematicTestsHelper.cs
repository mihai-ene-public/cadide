using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;

namespace IDE.Core.Presentation.Tests
{
    public static class SchematicTestsHelper
    {
        public static Symbol CreateVccSymbol()
        {
            var s = new Symbol
            {
                Items = new List<SchematicPrimitive>
                 {
                     new LineSchematic
                     {
                         x1 = 7.62,
                         y1 = 5.08,
                         x2 = 6.985,
                         y2 = 6.35,
                         width = 0.5,
                     },
                     new LineSchematic
                     {
                         x1 = 7.62,
                         y1 = 5.08,
                         x2 = 8.255,
                         y2 = 6.35,
                         width = 0.5,
                     },
                     new Pin
                     {
                         Name = "5V",
                         ShowName = false,
                         Number = "5V",
                         ShowNumber = false,
                         x = 7.62,
                         y = 7.62,
                         PinLength = 2.54,
                         Width = 0.5,
                         pinType = PinType.Power,
                         Orientation = pinOrientation.Down
                     }
                 }
            };

            return s;
        }

        public static SchematicSymbolCanvasItem CreatePart(Symbol symbol, double x, double y)
        {
            var part = new SchematicSymbolCanvasItem
            {
                X = x,
                Y = y,
                ShowName = false,
            };
            part.LoadSymbol(symbol);

            return part;
        }
    }
}
