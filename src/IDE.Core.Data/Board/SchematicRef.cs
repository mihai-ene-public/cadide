using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage
{
    //we should make the rule dealing with id and path
    //case id - found, path - file not exists
    //case id - not found, path - found
    // multiple id found (files were copy-pasted)
    // id-not found, path not found = there is a problem
    // both found: OK
    public class SchematicRef
    {
        [XmlAttribute("schematicId")]
        public long schematicId { get; set; }

        /// <summary>
        /// path relative to the project
        /// </summary>
        [XmlAttribute("hintPath")]
        public string hintPath { get; set; }
    }
}
