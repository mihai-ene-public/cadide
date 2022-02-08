using IDE.Core.Interfaces;

namespace IDE.Core.Toolbars
{
    public class FootprintToolbar : ToolbarModel
    {
        public FootprintToolbar(IFileBaseViewModel document)
        {
            Document = document;
        }
    }
}
