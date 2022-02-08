using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class Connect
    {
        /// <summary>
        /// pad number
        /// </summary>
        [XmlAttribute("pad")]
        public string pad
        {
            get; set;
        }


        [XmlAttribute("gateId")]
        public long gateId
        {
            get; set;
        }

        /// <summary>
        /// pin number
        /// </summary>
        [XmlAttribute("pin")]
        public string pin
        {
            get; set;
        }

    }
}
