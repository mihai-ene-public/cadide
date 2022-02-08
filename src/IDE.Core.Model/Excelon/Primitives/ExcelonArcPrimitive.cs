using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonArcPrimitive : ExcelonPrimitive
    {
        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        //stroke width
        public double Width { get; set; }

        public XSweepDirection SweepDirection { get; set; }

        //this is actual the radius
        public double SizeDiameter { get; set; }

        protected override void CreateTools()
        {
            cachedTools.Add( new NCTool { Diameter = Width });
        }

        //arcs not rendered properly
        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            if (cachedTools != null)
                excelonWriter.SelectTool(cachedTools[0].Number);
            excelonWriter.StartRoutingFrom(StartPoint.X, StartPoint.Y);
            excelonWriter.BeginRouting();
            if (SweepDirection == XSweepDirection.Clockwise)
                excelonWriter.RouteArcClockWiseTo(EndPoint.X, EndPoint.Y, SizeDiameter);
            else
                excelonWriter.RouteArcCounterClockWiseTo(EndPoint.X, EndPoint.Y, SizeDiameter);
            excelonWriter.RetractWithClamping();

            //var circDirection = CircularDirection.Clockwise;
            //if (SweepDirection == SweepDirection.Counterclockwise)
            //    circDirection = CircularDirection.CounterClockwise;

            //writer.SelectAperture(cachedApertures[0].Number);
            //writer.SetLevelPolarity(Polarity);
            //writer.SetCircularInterpolation(circDirection);
            //writer.SetCircularInterpolationMultiQuadrant();//?
            //writer.MoveTo(StartPoint.X, StartPoint.Y);
            //var offset = GetArcCenter();
            //writer.InterpolateArcTo(EndPoint.X, EndPoint.Y, offset.X, offset.Y);
        }

        //Point GetArcCenter()
        //{
        //    //https://math.stackexchange.com/questions/27535/how-to-find-center-of-an-arc-given-start-point-end-point-radius-and-arc-direc

        //    var sp = StartPoint;
        //    var ep = EndPoint;
        //    //distance between points
        //    var d = (sp - ep).Length;
        //    //distance of center from midpoint
        //    var h = Math.Sqrt(Math.Pow(SizeDiameter, 2.0d) - d * d / 4.0d);
        //    var dir = SweepDirection == XSweepDirection.Counterclockwise ? 1 : -1;
        //    var u = (ep.X - sp.X) / d;
        //    var v = (ep.Y - sp.Y) / d;
        //    var cx = 0.5 * (sp.X + ep.X) - dir * h * v;
        //    var cy = 0.5 * (sp.Y + ep.Y) + dir * h * u;
        //    cx -= sp.X;
        //    cy -= sp.Y;

        //    return new Point(cx, cy);
        //}
    }
}

