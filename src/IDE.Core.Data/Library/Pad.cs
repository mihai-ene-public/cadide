using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Pad : LayerPrimitive
    {
        public Pad()
        {
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

        [XmlAttribute("autoGenerateSolderMask")]
        public bool AutoGenerateSolderMask { get; set; } = true;

        [XmlAttribute("adjustSolderMaskInRules")]
        public bool AdjustSolderMaskInRules { get; set; } = true;

        [XmlAttribute("autoGeneratePasteMask")]
        public bool AutoGeneratePasteMask { get; set; } = true;

        [XmlAttribute("adjustPasteMaskInRules")]
        public bool AdjustPasteMaskInRules { get; set; } = true;

        //offset from the center of the rectangle
        [XmlAttribute("holeOffsetX")]
        public double HoleOffsetX { get; set; }

        [XmlAttribute("holeOffsetY")]
        public double HoleOffsetY { get; set; }


        [XmlAttribute("drillType")]
        public DrillType DrillType { get; set; } = DrillType.Drill;

        //hole size
        [XmlAttribute("drill")]
        public double drill
        {
            get; set;
        }

        [XmlAttribute("slotHeight")]
        public double SlotHeight { get; set; }

        [XmlAttribute("slotRot")]
        public double SlotRotation { get; set; }

        [XmlAttribute("plated")]
        public bool Plated
        {
            get; set;
        }

    }
}
