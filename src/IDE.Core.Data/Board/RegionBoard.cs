using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class RegionBoard
    {
        public RegionBoard()
        {
            Items = new List<BaseRegionPrimitive>();
        }

        /// <summary>
        /// the layer this region comes from
        /// </summary>
        [XmlAttribute("layerId")]
        public int LayerId { get; set; }

        [XmlAttribute("startPointX")]
        public double StartPointX { get; set; }

        [XmlAttribute("startPointY")]
        public double StartPointY { get; set; }

        [XmlAttribute("width")]
        public double Width { get; set; } = 0.2;

        //all items should be on the same layer
        [XmlElement("line", typeof(LineRegionPrimitive))]
        [XmlElement("arc", typeof(ArcRegionPrimitive))]
        public List<BaseRegionPrimitive> Items
        {
            get; set;
        }

        const double farPointX = 100d;
        const double farPointY = 80d;

        /// <summary>
        /// creates a rectangle region with size 100 x 80 mm
        /// </summary>
        public static RegionBoard CreateDefault()
        {


            var rb = new RegionBoard();
            rb.LayerId = (int)LayerType.BoardOutline + 1;

            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = farPointX,
                EndPointY = 0
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = farPointX,
                EndPointY = farPointY
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = 0,
                EndPointY = farPointY
            });
            rb.Items.Add(new LineRegionPrimitive
            {
                EndPointX = 0,
                EndPointY = 0
            });

            return rb;
        }

        public static List<LineBoard> GetDefaultBoardOutlineCanvasItems()
        {
            var layerId = (int)LayerType.BoardOutline + 1;
            return new List<LineBoard>
            {
                new LineBoard
                {
                    x1 = 0,
                    y1 = 0,
                    x2 = farPointX,
                    y2 = 0,
                    layerId = layerId
                },
                new LineBoard
                {
                    x1 = farPointX,
                    y1 = 0,
                    x2 = farPointX,
                    y2 = farPointY,
                    layerId = layerId
                },
                new LineBoard
                {
                    x1 = farPointX,
                    y1 = farPointY,
                    x2 = 0,
                    y2 = farPointY,
                    layerId = layerId
                },
                new LineBoard
                {
                    x1 = 0,
                    y1 = farPointY,
                    x2 = 0,
                    y2 = 0,
                    layerId = layerId
                }
            };
        }
    }

    public class BaseRegionPrimitive : IRegionPrimitive
    {
        [XmlAttribute("endPointX")]
        public double EndPointX { get; set; }

        [XmlAttribute("endPointY")]
        public double EndPointY { get; set; }


    }

    public class LineRegionPrimitive : BaseRegionPrimitive
    {

    }

    public class ArcRegionPrimitive : BaseRegionPrimitive
    {
        [XmlAttribute("sizeDiameter")]
        public double SizeDiameter { get; set; } = 3.0d;

        [XmlAttribute("sweepDirection")]
        public XSweepDirection SweepDirection { get; set; }

        [XmlAttribute("isLargeArc")]
        public bool IsLargeArc { get; set; }
    }
}
