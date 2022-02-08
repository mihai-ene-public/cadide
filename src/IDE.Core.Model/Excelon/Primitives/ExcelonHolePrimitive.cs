using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonHolePrimitive : ExcelonPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Diameter { get; set; }

        public DrillPlating Plating { get; set; } = DrillPlating.Plated;

        protected override void CreateTools()
        {
            cachedTools.Add(new NCTool { Diameter = Diameter, Plating = Plating });
        }

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            if (cachedTools != null)
                excelonWriter.SelectTool(cachedTools[0].Number);
            excelonWriter.DrillAt(X, Y);
        }
    }
}
