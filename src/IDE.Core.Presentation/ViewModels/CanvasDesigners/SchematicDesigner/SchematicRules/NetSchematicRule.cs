using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Designers
{
    /// <summary>
    /// An abstract rule that applies to a net
    /// </summary>
    public abstract class NetSchematicRule : AbstractSchematicRule
    {
        SchematicRuleResponse ruleResponse ;
        public SchematicRuleResponse RuleResponse
        {
            get
            {
                return ruleResponse;
            }
            set
            {
                ruleResponse = value;
                OnPropertyChanged(nameof(RuleResponse));
            }
        }


        public abstract SchematicRuleCheckResult CheckNet(SchematicNet net);
    }
}
