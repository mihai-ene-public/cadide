using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Variant
    {
        public Variant()
        {
            populate = true;
        }

        [XmlAttribute()]
        public string name
        {
            get; set;
        }


        [XmlAttribute()]
        public bool populate
        {
            get; set;
        }


        [XmlAttribute()]
        public string value
        {
            get; set;
        }


    }
}
