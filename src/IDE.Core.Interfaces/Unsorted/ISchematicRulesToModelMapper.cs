namespace IDE.Core.Interfaces
{
    public interface ISchematicRulesToModelMapper : IService
    {
        ISchematicRuleModel CreateRuleItem(ISchematicRuleData rule);

    }
}
