using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //a footprint instance
    /*
     * To solve the footprint we need the id and the library
     * Component is not needed
     */ 
    public class BoardComponentInstance : LayerPrimitive//todo this should higher than a primitive (something that inherits from Primitive)
    {
        public BoardComponentInstance()
        {
            PartNameFontSize = 1;
            PartNameFontFamily = "Arial";
        }

        //id of this instance
        [XmlAttribute("id")]
        public long Id { get; set; }

        [XmlAttribute("partId")]
        public long PartId { get; set; }

        //part name /designator
        [XmlAttribute("partName")]
        public string PartName { get; set; }

        /// <summary>
        /// the library name the component for this footprint belongs to
        /// </summary>
        [XmlAttribute("compLibrary")]
        public string ComponentLibrary { get; set; }

        /// <summary>
        /// componentId this footprint belongs to
        /// </summary>
        [XmlAttribute("compId")]
        public long ComponentId { get; set; }


        /// <summary>
        /// Library name of the footprint. Needed for solving
        /// </summary>
        [XmlAttribute("library")]
        public string Library
        {
            get; set;
        }

        /// <summary>
        /// the id of the footprint from the component. Needed for solving
        /// </summary>
        [XmlAttribute("footprintId")]
        public long FootprintId { get; set; }

        /* These might not be needed
        [XmlAttribute("compLibrary")]
        public string ComponentLibrary { get; set; }

        [XmlAttribute("compId")]
        public long ComponentId { get; set; }
        */


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

        //position for part name/deignator (U1, Q1, R2, etc)
        [XmlAttribute("partNameX")]
        public double PartNameX { get; set; }

        [XmlAttribute("partNameY")]
        public double PartNameY { get; set; }

        [XmlAttribute("partNameRot")]
        public double PartNameRot { get; set; }

        [XmlAttribute("showName")]
        public bool ShowName { get; set; } = true;


        [XmlAttribute("partNameFontFamily")]
        public string PartNameFontFamily { get; set; }

        [XmlAttribute("partNameFontSize")]
        public double PartNameFontSize { get; set; }

        [XmlAttribute("partNameStrokeWidth")]
        public double PartNameStrokeWidth { get; set; } = 0.2;

        [XmlAttribute("placement")]
        public FootprintPlacement Placement { get; set; }

        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

    }


}
