using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public enum dimensionDtype
    {
        parallel,
        horizontal,
        vertical,
        radius,
        diameter,
        //leader,
    }

    public enum dimensionUnit
    {
       // mic,
        mm,
        mil,
        //inch,
    }

    /// <summary>
    /// Measures shown canvas.
    /// </summary>
    public class Dimension
    {
        public Dimension()
        {
            dtype = dimensionDtype.parallel;
            unit = dimensionUnit.mm;
            precision = 2;
        }

        [XmlAttribute()]
        public double x1
        {
            get; set;
        }


        [XmlAttribute()]
        public double y1
        {
            get; set;
        }


        [XmlAttribute()]
        public double x2
        {
            get; set;
        }


        [XmlAttribute()]
        public double y2
        {
            get; set;
        }


        [XmlAttribute()]
        public double x3
        {
            get; set;
        }


        [XmlAttribute()]
        public double y3
        {
            get; set;
        }


        [XmlAttribute()]
        public int layer
        {
            get; set;
        }


        [XmlAttribute()]
        public dimensionDtype dtype
        {
            get; set;
        }


        [XmlAttribute()]
        public double width
        {
            get; set;
        }

        [XmlAttribute()]
        public double extwidth
        {
            get; set;
        }

        [XmlAttribute()]
        public double extlength
        {
            get; set;
        }


        [XmlAttribute()]
        public double extoffset
        {
            get; set;
        }


        [XmlAttribute()]
        public double textsize
        {
            get; set;
        }


        [XmlAttribute()]
        public int textratio
        {
            get; set;
        }


        [XmlAttribute()]
        public dimensionUnit unit
        {
            get; set;
        }


        [XmlAttribute()]
        public int precision
        {
            get; set;
        }


        [XmlAttribute()]
        public bool visible
        {
            get; set;
        }
    }
}
