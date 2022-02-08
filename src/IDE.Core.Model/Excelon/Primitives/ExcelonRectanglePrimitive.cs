using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{

    //this should be a flash
    public class ExcelonRectanglePrimitive : ExcelonPrimitive
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        //this is mandatory
        public double BorderWidth { get; set; }

        protected override void CreateTools()
        {
            cachedTools.Add(new NCTool
            {
                Diameter = BorderWidth
            });
        }

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            //if (cachedTools != null)
            //    excelonWriter.SelectTool(cachedTools[0].Number);
            //excelonWriter.SetLevelPolarity(Polarity);
            //excelonWriter.FlashApertureTo(X + 0.5 * Width, Y + 0.5 * Height);
        }
    }
}
