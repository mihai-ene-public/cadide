using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Common.Geometries
{
    public class RectangleGeometryOutline : GeometryOutline
    {
        public RectangleGeometryOutline(double centerX, double centerY, double width, double height, double cornerRadius = 0.0, int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
        {
            //outline goes clockwise

            //top-left coordinates

            if (cornerRadius != 0.0)
            {
               

                var numberSegments = numberOfSegmentsPerCircle / 4;
                //top left corner
                points.AddRange(GetCirclePoints(new XPoint(centerX - 0.5 * width + cornerRadius, centerY - 0.5 * height + cornerRadius)
                                               , numberSegments
                                               , Math.PI
                                               , 0.5 * Math.PI
                                               , cornerRadius
                                               , false
                                                ));

                //top-right corner
                points.AddRange(GetCirclePoints(new XPoint(centerX + 0.5 * width - cornerRadius, centerY - 0.5 * height + cornerRadius)
                                               , numberSegments
                                               , 0.5 * Math.PI
                                               , 0.5 * Math.PI
                                               , cornerRadius
                                               , false
                                                ));

                //bottom right corner
                points.AddRange(GetCirclePoints(new XPoint(centerX + 0.5 * width - cornerRadius, centerY + 0.5 * height - cornerRadius)
                                               , numberSegments
                                               , angleStart: 0
                                               , totalAngle: 0.5 * Math.PI
                                               , radius: cornerRadius
                                               , counterClockwise: false
                                                ));

                //bottom left corner
                points.AddRange(GetCirclePoints(new XPoint(centerX - 0.5 * width + cornerRadius, centerY + 0.5 * height - cornerRadius)
                                               , numberSegments
                                               , angleStart: -0.5 * Math.PI
                                               , totalAngle: 0.5 * Math.PI
                                               , radius: cornerRadius
                                               , counterClockwise: false
                                                ));

                //add first point to close
                points.Add(points[0]);
            }
            else
            {
                points.Add(new XPoint(centerX - 0.5 * width, centerY - 0.5 * height));
                points.Add(new XPoint(centerX + 0.5 * width, centerY - 0.5 * height));
                points.Add(new XPoint(centerX + 0.5 * width, centerY + 0.5 * height));
                points.Add(new XPoint(centerX - 0.5 * width, centerY + 0.5 * height));
                points.Add(points[0]);//last point like the first to close the loop
            }
        }


    }
}
