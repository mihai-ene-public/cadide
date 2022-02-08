using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonCirclePrimitive : ExcelonPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Diameter { get; set; }

        public double BorderWidth { get; set; }

        protected override void CreateTools()
        {
            cachedTools.Add(new NCTool { Diameter = BorderWidth });
        }

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            if (cachedTools != null)
                excelonWriter.SelectTool(cachedTools[0].Number);
            var radius = 0.5 * Diameter;
            var sp = new XPoint(X - radius, Y);
            var ep = new XPoint(X + radius, Y);

            excelonWriter.StartRoutingFrom(sp.X, sp.Y);
            excelonWriter.BeginRouting();
            excelonWriter.RouteArcClockWiseTo(ep.X, ep.Y, radius);
            excelonWriter.RouteArcClockWiseTo(sp.X, sp.Y, radius);
            excelonWriter.RetractWithClamping();
        }
    }
}

