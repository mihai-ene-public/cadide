using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public enum portDirection
    {


        nc,


        input,


        output,


        io,


        oc,


        pwr,


        pas,


        hiz,
    }

    public class Port
    {
        public Port()
        {
            Direction = portDirection.io;
        }


        [XmlAttribute("name")]
        public string Name
        {
            get; set;
        }


        [XmlAttribute("side")]
        public int Side
        {
            get; set;
        }


        [XmlAttribute("coord")]
        public double Coord
        {
            get; set;
        }


        [XmlAttribute("direction")]
        public portDirection Direction
        {
            get; set;
        }
    }
}
