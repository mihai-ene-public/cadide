using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Model.Gerber.Primitives.Apertures;


namespace IDE.Core.Gerber
{

    //this should be a flash
    public class GerberRectanglePrimitive : GerberPrimitive
    {

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double CornerRadius { get; set; }

        public double Rot { get; set; }

        protected override void CreateApertures()
        {
            cachedApertures.Add(new ApertureDefinitionRotatedRoundedRectangle
            {
                Width = Width,
                Height = Height,
                Rot = Rot,
                CornerRadius = CornerRadius
            });
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
