using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    public class ProjectProperties //: IProjectProperties
    {
        #region Pack

        public string Id { get; set; }

        public string Version { get; set; }
        public string Authors { get; set; }
        public string License { get; set; }
        public string LicenseUrl { get; set; }
        public string Icon { get; set; }
        public string ProjectUrl { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public string Tags { get; set; }

        #endregion Pack

        #region Build

        public string BuildOutputFolderPath{get;set;} = "!Output";

        //set only for library
        public string BuildOutputFileName { get; set; }

        public string BuildOutputNamespace { get; set; }

        #endregion

        /// <summary>
        /// Project variables
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<Property> Properties { get; set; }
    }
}
