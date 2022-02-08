using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    //represents a region, a poly
    public class ExcelonRegionPrimitive : ExcelonPrimitive
    {
        //public Point StartPoint { get; set; }

        public List<ExcelonPrimitive> RegionItems { get; set; } = new List<ExcelonPrimitive>();

        public override void WriteExcelon(NCDrillFileWriter excelonWriter)
        {
            //gerberWriter.SetLevelPolarity(Polarity);
            //gerberWriter.SetRegionModeStart();

            //foreach (var item in RegionItems)
            //{
            //    if(item is GerberLinePrimitive)
            //    {
            //        var line = item as GerberLinePrimitive;
            //        gerberWriter.SetLinearInterpolation();
            //        gerberWriter.MoveTo(line.StartPoint.X, line.StartPoint.Y);
            //        gerberWriter.InterpolateTo(line.EndPoint.X, line.EndPoint.Y);
            //    }
            //    else if (item is GerberArcPrimitive)
            //    {
            //        var arc = item as GerberArcPrimitive;
            //        var circDirection = CircularDirection.Clockwise;
            //        if (arc.SweepDirection == SweepDirection.Counterclockwise)
            //            circDirection = CircularDirection.CounterClockwise;

            //        gerberWriter.SetCircularInterpolation(circDirection);
            //        gerberWriter.SetCircularInterpolationMultiQuadrant();//?
            //        gerberWriter.MoveTo(arc.StartPoint.X, arc.StartPoint.Y);
            //        gerberWriter.InterpolateArcTo(arc.EndPoint.X, arc.EndPoint.Y, arc.SizeDiameter, arc.SizeDiameter);
            //    }
            //}

            //gerberWriter.SetRegionModeEnd();
               
        }
    }
}
