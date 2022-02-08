using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Designers
{
    public class SchematicRuleCheckResult: RuleCheckResult
    {
        public SchematicRuleResponse CheckResponse { get; set; }
    }
}
