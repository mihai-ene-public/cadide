using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /// <summary>
    /// Footprint definition
    /// </summary>
    [XmlRoot("footprint")]
    public class Footprint : LibraryItem
    {
        [XmlArray("layers")]
        [XmlArrayItem("layer")]
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// Description serving as comment for documentation
        /// </summary>
        [XmlElement("description")]
        public Description Description { get; set; }

        [XmlElement("circle", typeof(CircleBoard))]
        //[XmlElement("ellipse", typeof(EllipseBoard))]
        [XmlElement("hole", typeof(Hole))]
        [XmlElement("pad", typeof(Pad))]
        [XmlElement("poly", typeof(PolygonBoard))]
        [XmlElement("rect", typeof(RectangleBoard))]
        [XmlElement("smd", typeof(Smd))]//
        [XmlElement("text", typeof(TextBoard))]
        [XmlElement("mono", typeof(TextSingleLineBoard))]
        [XmlElement("line", typeof(LineBoard))]
        [XmlElement("arc", typeof(ArcBoard))]
        public List<LayerPrimitive> Items
        {
            get; set;
        }


        [XmlElement("model")]
        public List<ModelData> Models { get; set; } = new List<ModelData>();


        public static List<Layer> CreateDefaultLayers()
        {
            return new List<Layer>
            {
               Layer.GetTopOverlayLayer(),
               Layer.GetTopPasteLayer(),
               Layer.GetTopSolderLayer(),
               Layer.GetTopLayer(),
               Layer.GetTopMechanicalLayer(),
               Layer.GetMillingLayer(),
            };
        }
    }



}
