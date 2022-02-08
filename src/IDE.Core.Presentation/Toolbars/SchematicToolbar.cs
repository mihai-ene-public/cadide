using IDE.Core.Interfaces;

namespace IDE.Core.Toolbars
{
    public class SchematicToolbar : ToolbarModel
    {
        public SchematicToolbar(IFileBaseViewModel document)
        {
            Document = document;
        }
    }
}
