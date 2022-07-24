using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Model.Gerber.Primitives.Apertures;

namespace IDE.Core.Gerber
{
    public class GerberCirclePrimitive : GerberPrimitive
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Diameter { get; set; }

        protected override void CreateApertures()
        {
            cachedApertures.Add(new ApertureDefinitionCircle { Diameter = Diameter });
        }

        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            if (cachedApertures != null)
                gerberWriter.SelectAperture(cachedApertures[0].Number);
            gerberWriter.SetLevelPolarity(Polarity);
            gerberWriter.FlashApertureTo(X, Y);
        }
    }
}

