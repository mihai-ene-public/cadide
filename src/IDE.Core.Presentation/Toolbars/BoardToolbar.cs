using IDE.Core.Interfaces;

namespace IDE.Core.Toolbars
{
    public class BoardToolbar:ToolbarModel
    {
        public BoardToolbar(IFileBaseViewModel document)
        {
            Document = document;
        }
    }
}
