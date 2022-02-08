using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonLinePrimitive : ExcelonPrimitive
    {
        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        public double Width { get; set; }

        protected override void CreateTools()
        {
            cachedTools.Add( new NCTool { Diameter = Width });
        }

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            if (cachedTools != null)
                excelonWriter.SelectTool(cachedTools[0].Number);
            excelonWriter.StartRoutingFrom(StartPoint.X, StartPoint.Y);
            excelonWriter.BeginRouting();
            excelonWriter.RouteLinearTo(EndPoint.X, EndPoint.Y);
            excelonWriter.RetractWithClamping();

            //excelonWriter.SetLevelPolarity(Polarity);
            //excelonWriter.SetLinearInterpolation();
            //excelonWriter.MoveTo(StartPoint.X, StartPoint.Y);
            //excelonWriter.InterpolateTo(EndPoint.X, EndPoint.Y);
        }
    }
}
