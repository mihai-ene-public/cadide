using IDE.Core.Types.Media;
using System.Collections.Generic;
using IDE.Core.Model.Gerber.Primitives.Apertures;


namespace IDE.Core.Gerber
{
    public class GerberPolylinePrimitive : GerberPrimitive
    {
        public double Width { get; set; }

        public List<XPoint> Points { get; set; }

        protected override void CreateApertures()
        {
            cachedApertures.Add(new ApertureDefinitionCircle { Diameter = Width });
        }

        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            if (Points.Count > 1)
            {
                if (cachedApertures != null)
                    gerberWriter.SelectAperture(cachedApertures[0].Number);
                gerberWriter.SetLevelPolarity(Polarity);
                gerberWriter.SetLinearInterpolation();

                var sp = Points[0];
                gerberWriter.MoveTo(sp.X, sp.Y);

                for (int i = 1; i < Points.Count; i++)
                    gerberWriter.InterpolateTo(Points[i].X, Points[i].Y);
            }
        }
    }
}
