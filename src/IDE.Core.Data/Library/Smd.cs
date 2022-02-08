using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{


    public class Smd : LayerPrimitive
    {
        public Smd()
        {
            //stop = false;
            //thermals = true;
            //cream = true;

            //Orientation = SmdOrientation.Horizontal;

        }

        [XmlAttribute("number")]
        public string number { get; set; }

        [XmlAttribute("layer")]
        public int layerId { get; set; } = LayerConstants.SignalTopLayerId;




        [XmlAttribute("x")]
        public double x
        {
            get; set;
        }


        [XmlAttribute("y")]
        public double y
        {
            get; set;
        }

        [XmlAttribute("rot")]
        public double rot
        {
            get; set;
        }

        //x- sixe
        [XmlAttribute("width")]
        public double Width
        {
            get; set;
        }

        //y size
        [XmlAttribute("height")]
        public double Height
        {
            get; set;
        }

        [XmlAttribute("cornerRadius")]
        public double CornerRadius { get; set; }

        //[XmlAttribute]
        //public double PasteMaskExpansion { get; set; }

        //[XmlAttribute]
        //public double SolderMaskExpansion { get; set; }

        [XmlAttribute("autoGenerateSolderMask")]
        public bool AutoGenerateSolderMask { get; set; } = true;

        [XmlAttribute("adjustSolderMaskInRules")]
        public bool AdjustSolderMaskInRules { get; set; } = true;

        [XmlAttribute("autoGeneratePasteMask")]
        public bool AutoGeneratePasteMask { get; set; } = true;

        [XmlAttribute("adjustPasteMaskInRules")]
        public bool AdjustPasteMaskInRules { get; set; } = true;

    }
}
