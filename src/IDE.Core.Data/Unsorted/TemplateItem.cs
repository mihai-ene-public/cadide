using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    [XmlRoot("Template")]
    public class TemplateItem
    {
        [XmlIgnore]
        public string FilePath { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Extension name without dot (.)
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// local path to the icon (relative to the template file)
        /// </summary>
        public string Icon { get; set; }

        [XmlAttribute("Type")]
        public TemplateType TemplateType { get; set; }


    }

}
