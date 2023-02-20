using IDE.Core.Interfaces;

namespace IDE.Documents.Views;

public interface ITemplateRepository
{
    IList<TemplateItemInfo> LoadTemplates(TemplateType templateType);

}
