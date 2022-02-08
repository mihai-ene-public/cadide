using IDE.Core.Interfaces;
using System;
using System.Text;
using System.Xml.Serialization;


namespace IDE.Core.Storage
{
    public abstract class SchematicRuleData : ISchematicRuleData
    {
        [XmlAttribute("isEnabled")]
        public bool IsEnabled { get; set; }
    }
}
