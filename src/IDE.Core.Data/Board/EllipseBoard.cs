using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    /*
    public class EllipseBoard : LayerPrimitive, ILayerPrimitive, IEllipse
    {
        public EllipseBoard()
        {
            BorderWidth = 10;
        }

        /// <summary>
        /// center X
        /// </summary>
        [XmlAttribute]
        public double x
        {
            get; set;
        }

        /// <summary>
        /// center Y
        /// </summary>
        [XmlAttribute]
        public double y
        {
            get; set;
        }

        [XmlAttribute]
        public double Width { get; set; }

        [XmlAttribute]
        public double Height { get; set; }

        [XmlAttribute]
        public double BorderWidth
        {
            get; set;
        }

        [XmlAttribute]
        public bool IsFilled { get; set; }

        [XmlAttribute("layer")]
        public int layerId
        {
            get; set;
        }

        public override BaseCanvasItem CreateDesignerItem(IList<LayerDesignerItem> documentLayers, FootprintPlacement placement = FootprintPlacement.Top)
        {
            var w = new EllipseBoardCanvasItem();
            w.Primitive = this;
            w.LoadLayers(documentLayers);
            return w;
        }
    }
    */
}
