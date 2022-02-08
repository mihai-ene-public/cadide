using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class TrackBoard : LayerPrimitive, ILayerPrimitive
    {

        [XmlAttribute("width")]
        public double width { get; set; } = 0.2;

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

        //[XmlElement("vertex")]
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
