using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class NetWire : NetSegmentItem//Wire
    {
        public NetWire() : base()
        {
            LineColor = "#FF0000FF";
        }

        [XmlAttribute("x1")]
        public double X1
        {
            get; set;
        }

        [XmlIgnore]
        public bool X1Specified => Points.Count == 0;


        [XmlAttribute("y1")]
        public double Y1
        {
            get; set;
        }

        [XmlIgnore]
        public bool Y1Specified => Points.Count == 0;

        [XmlAttribute("x2")]
        public double X2
        {
            get; set;
        }

        [XmlIgnore]
        public bool X2Specified => Points.Count == 0;

        [XmlAttribute("y2")]
        public double Y2
        {
            get; set;
        }

        [XmlIgnore]
        public bool Y2Specified => Points.Count == 0;

        /// <summary>
        /// thickness in mm
        /// </summary>
        [XmlAttribute("width")]
        public double Width
        {
            get; set;
        }

        [XmlAttribute("lineColor")]
        public string LineColor { get; set; }

        [XmlIgnore]
        public List<Vertex> Points { get; set; } = new List<Vertex>();

        //don't use this on code when loading or saving
        [XmlElement("vertices")]
        public string VerticesString
        {
            get
            {
                return string.Join(" ", Points.Select(p => $"{p.x},{p.y}"));
            }
            set
            {
                var pointsStrings = value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Points = new List<Vertex>();

                foreach (var pointStr in pointsStrings)
                {
                    var cstr = pointStr.Split(',');
                    if (cstr.Length == 2)
                    {
                        var x = double.Parse(cstr[0], CultureInfo.InvariantCulture);
                        var y = double.Parse(cstr[1], CultureInfo.InvariantCulture);

                        Points.Add(new Vertex { x = x, y = y });
                    }
                }
            }

        }
    }
}
