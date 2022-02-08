using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonFigure : ExcelonPrimitive
    {
      //  public Point StartPoint { get; set; }

        public List<ExcelonPrimitive> FigureItems { get; set; } = new List<ExcelonPrimitive>();

        protected override void CreateTools()
        {
            foreach (var item in FigureItems)
                cachedTools.AddRange(item.GetTools());
        }

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            foreach (var item in FigureItems)
                item.WriteExcelon(excelonWriter);
        }
    }
}
