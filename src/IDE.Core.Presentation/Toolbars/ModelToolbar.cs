using IDE.Core.Interfaces;
namespace IDE.Core.Toolbars
{
    public class ModelToolbar : ToolbarModel
    {
        public ModelToolbar(IFileBaseViewModel document)
        {
            Document = document;
        }
    }
}
