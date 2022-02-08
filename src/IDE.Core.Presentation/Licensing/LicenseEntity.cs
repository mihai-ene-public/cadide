using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace IDE.Core.Presentation.Licensing
{
    public class LicenseEntity
    {
        [XmlElement("AppName")]
        public string AppName { get; set; }

        /// <summary>
        /// Hardware ID
        /// </summary>
        [XmlElement("UID")]
        public string HardwareId { get; set; }

        [XmlElement("Type")]
        public LicenseType LicenseType { get; set; }

        [XmlElement("CreateDateTime")]
        public DateTime CreateDateTime { get; set; }


    }


}
