using IDE.Core.Model.Gerber.Primitives.Apertures;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    public class GerberArcPrimitive : GerberPrimitive
    {
        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        //stroke width
        public double Width { get; set; }

        public XSweepDirection SweepDirection { get; set; }

        public double SizeDiameter { get; set; }

        protected override void CreateApertures()
        {
            cachedApertures.Add(new ApertureDefinitionCircle { Diameter = Width });
        }

        //arcs not rendered properly
        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            var circDirection = CircularDirection.Clockwise;
            if (SweepDirection == XSweepDirection.Counterclockwise)
                circDirection = CircularDirection.CounterClockwise;

            gerberWriter.SelectAperture(cachedApertures[0].Number);
            gerberWriter.SetLevelPolarity(Polarity);
            gerberWriter.SetCircularInterpolation(circDirection);
            gerberWriter.SetCircularInterpolationMultiQuadrant();//?
            gerberWriter.MoveTo(StartPoint.X, StartPoint.Y);
            var center = GetArcCenter();

            //this below doesn't work; because it is expected that coordinates to be top-left, but Gerber coords are cartezian
            //var center = IDE.Core.GeometryExtensions.GetArcCenter(StartPoint, EndPoint, 0.5 * SizeDiameter, 0.5 * SizeDiameter, SweepDirection);

            var offset = center - StartPoint;
            gerberWriter.InterpolateArcTo(EndPoint.X, EndPoint.Y, offset.X, offset.Y);
        }

        ////todo: add this to a helper
        internal XPoint GetArcCenter()
        {
            //https://math.stackexchange.com/questions/27535/how-to-find-center-of-an-arc-given-start-point-end-point-radius-and-arc-direc

            var sp = StartPoint;
            var ep = EndPoint;
            //distance between points
            var d = (sp - ep).Length;

            //n(u,v) - unit normal in the direction sp->ep
            var u = (ep.X - sp.X) / d;
            var v = (ep.Y - sp.Y) / d;

            //n* (-v, u) - unit vector you get by rotating n counterclockwise by 90 deg
            var nu = -v;
            var nv = u;

            //midpoint
            var m = new XPoint(0.5 * (sp.X + ep.X), 0.5 * (sp.Y + ep.Y));

            //distance of center from midpoint
            var radius = 0.5 * SizeDiameter;
            //var maxDist = Math.Max(d, radius);
            //var h = Math.Sqrt(Math.Pow(maxDist, 2.0d) - d * d / 4.0d);//was SizeDiameter

            //h - distance between center and midpoint m
            var h = Math.Sqrt(radius * radius - (d * d / 4.0d));

            if (double.IsNaN(h))
                h = 0.0d;
            var dir = SweepDirection == XSweepDirection.Counterclockwise ? 1 : -1;

            //center: c = m + dir * h * n*
            var cx = m.X + dir * h * nu;
            var cy = m.Y + dir * h * nv;


            //var cx = 0.5 * (sp.X + ep.X) - dir * h * v;
            //var cy = 0.5 * (sp.Y + ep.Y) + dir * h * u;
            //cx -= sp.X;
            //cy -= sp.Y;


            return new XPoint(cx, cy);
        }
    }
}

