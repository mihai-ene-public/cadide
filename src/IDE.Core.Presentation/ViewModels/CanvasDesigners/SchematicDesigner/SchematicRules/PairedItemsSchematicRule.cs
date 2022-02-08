using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public abstract class PairedItemsSchematicRule : AbstractSchematicRule
    {
        public abstract SchematicRuleCheckResult CheckItems(ISelectableItem item1, ISelectableItem item2);
    }
}
