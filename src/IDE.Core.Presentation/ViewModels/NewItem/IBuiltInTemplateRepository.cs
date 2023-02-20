namespace IDE.Documents.Views;

public interface IBuiltInTemplateRepository : ITemplateRepository
{
    void CreateItemFromTemplate(BuiltInTemplateItemInfo templateItemInfo, string itemFilePath);
}
