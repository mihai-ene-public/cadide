using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class PadRef : Primitive, IPadRef
    {

        [XmlAttribute("footprintInstanceId")]
        public long FootprintInstanceId { get; set; }

        [XmlAttribute("padNumber")]
        public string PadNumber { get; set; }

    }
}
