using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class PortRef
    {
        [XmlAttribute()]
        public string moduleinst
        {
            get; set;
        }


        [XmlAttribute()]
        public string port
        {
            get; set;
        }
    }
}
