using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class GerberFigure : GerberPrimitive
    {
      //  public Point StartPoint { get; set; }

        public List<GerberPrimitive> FigureItems { get; set; } = new List<GerberPrimitive>();

        protected override void CreateApertures()
        {
            foreach (var item in FigureItems)
                cachedApertures.AddRange(item.GetApertures());
        }

        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            foreach (var item in FigureItems)
                item.WriteGerber(gerberWriter);
        }
    }
}
