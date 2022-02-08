using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{



    //name and number(designator)
    //color will come from schema color (all pins will have same color)
    public class Pin : SchematicPrimitive
    {
        public Pin()
        {
            //PinLength = pinLength.Short;
            PinLength = 3;
            Width = 0.5;
            pinType = PinType.Passive;
            Orientation = pinOrientation.Right;
            PinColor = "#FF000080";
            PinNameColor = "#FF000080";
            PinNumberColor = "#FF000080";
        }

        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }

        [XmlAttribute("showName")]
        public bool ShowName { get; set; } = true;


        [XmlAttribute("pinNameX")]
        public double PinNameX { get; set; } = 1;

        [XmlAttribute("pinNameY")]
        public double PinNameY { get; set; } = -1.45d;

        [XmlAttribute("pinNameRot")]
        public double PinNameRot { get; set; }

        [XmlAttribute("number")]
        public string Number { get; set; }

        [XmlAttribute("showNumber")]
        public bool ShowNumber { get; set; }

        [XmlAttribute("pinNumberX")]
        public double PinNumberX { get; set; } = 0.3d;

        [XmlAttribute("pinNumberY")]
        public double PinNumberY { get; set; } = -2.25d;

        [XmlAttribute("pinNumberRot")]
        public double PinNumberRot { get; set; }

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

        [XmlAttribute("pinLength")]
        public double PinLength
        {
            get; set;
        }

        [XmlAttribute("width")]
        public double Width
        {
            get; set;
        }


        [XmlAttribute("pinType")]
        public PinType pinType
        {
            get; set;
        }

        [XmlAttribute("orientation")]
        public pinOrientation Orientation { get; set; }


        [XmlAttribute("swapLevel")]
        public int swaplevel
        {
            get; set;
        }

        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [XmlAttribute("pinColor")]
        public string PinColor { get; set; }

        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [XmlAttribute("pinNameColor")]
        public string PinNameColor { get; set; }

        /// <summary>
        /// color for the line of the pin
        /// </summary>
        [XmlAttribute("pinNumberColor")]
        public string PinNumberColor { get; set; }

    }
}
