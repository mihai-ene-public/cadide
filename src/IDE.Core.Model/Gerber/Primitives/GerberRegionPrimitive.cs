using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Gerber
{
    //represents a region, a poly
    public class GerberRegionPrimitive : GerberPrimitive
    {
        //public Point StartPoint { get; set; }

        public List<GerberPrimitive> RegionItems { get; set; } = new List<GerberPrimitive>();

        public override void WriteGerber(Gerber274XWriter gerberWriter)
        {
            gerberWriter.SetLevelPolarity(Polarity);
            gerberWriter.SetRegionModeStart();

            //todo: we need to use the WriteGerberMethod of the primitive itself but take into account we are in region mode
            foreach (var item in RegionItems)
            {
                if (item is GerberLinePrimitive)
                {
                    var line = item as GerberLinePrimitive;
                    gerberWriter.SetLinearInterpolation();
                    gerberWriter.MoveTo(line.StartPoint.X, line.StartPoint.Y);
                    gerberWriter.InterpolateTo(line.EndPoint.X, line.EndPoint.Y);
                }
                else if (item is GerberArcPrimitive arc)
                {
                    // var arc = item as GerberArcPrimitive;
                    var circDirection = CircularDirection.Clockwise;
                    if (arc.SweepDirection == XSweepDirection.Counterclockwise)
                        circDirection = CircularDirection.CounterClockwise;

                    gerberWriter.SetCircularInterpolation(circDirection);
                    gerberWriter.SetCircularInterpolationMultiQuadrant();//?
                    gerberWriter.MoveTo(arc.StartPoint.X, arc.StartPoint.Y);

                    var center = arc.GetArcCenter();
                    var offset = center - arc.StartPoint;
                    gerberWriter.InterpolateArcTo(arc.EndPoint.X, arc.EndPoint.Y, offset.X, offset.Y);
                }
            }

            gerberWriter.SetRegionModeEnd();

        }
    }
}
