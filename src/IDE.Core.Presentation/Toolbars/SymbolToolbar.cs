using IDE.Core.Interfaces;

namespace IDE.Core.Toolbars
{
    public class SymbolToolbar : ToolbarModel
    {
        public SymbolToolbar(IFileBaseViewModel document)
        {
            Document = document;    
        }
    }
}
