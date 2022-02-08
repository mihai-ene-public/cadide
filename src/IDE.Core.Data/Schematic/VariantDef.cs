using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class VariantDef
    {
        [XmlAttribute()]
        public string name
        {
            get; set;
        }


        [XmlAttribute()]
        public bool current
        {
            get; set;
        }
    }
}
