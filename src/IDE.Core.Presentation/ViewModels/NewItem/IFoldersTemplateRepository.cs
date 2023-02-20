namespace IDE.Documents.Views;

public interface IFoldersTemplateRepository : ITemplateRepository
{
    void CreateItemFromTemplate(string templateFilePath, string itemFilePath, bool isProjectTemplate);
}
