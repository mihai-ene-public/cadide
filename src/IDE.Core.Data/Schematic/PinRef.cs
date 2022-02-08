using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{

    public class PinRef : NetSegmentItem
    {

        [XmlAttribute("partGateId")]
        public long PartInstanceId { get; set; }

        /// <summary>
        /// pin number
        /// </summary>
        [XmlAttribute("pin")]
        public string Pin { get; set; }

    }
}
