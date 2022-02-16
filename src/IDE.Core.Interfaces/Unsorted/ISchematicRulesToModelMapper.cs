namespace IDE.Core.Interfaces
{
    public interface ISchematicRulesToModelMapper
    {
        ISchematicRuleModel CreateRuleItem(ISchematicRuleData rule);

    }
}
