using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Designers
{
    public abstract class SingleItemSchematicRule : AbstractSchematicRule
    {
        SchematicRuleResponse ruleResponse;
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
        public abstract SchematicRuleCheckResult CheckItem(ISelectableItem item);

    }
}
