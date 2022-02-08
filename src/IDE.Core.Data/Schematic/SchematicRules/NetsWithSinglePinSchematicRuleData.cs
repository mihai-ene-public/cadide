using System.Xml.Serialization;


namespace IDE.Core.Storage
{
    public class NetsWithSinglePinSchematicRuleData : SchematicRuleData
    {
        [XmlAttribute("ruleResponse")]
        public SchematicRuleResponse RuleResponse { get; set; }
    }
}
