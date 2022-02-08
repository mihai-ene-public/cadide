using IDE.Core.Interfaces;

namespace IDE.Core.Toolbars
{
    /// <summary>
    /// base class for toolbars
    /// </summary>
    public abstract class ToolbarModel : BaseViewModel
    {

        public IFileBaseViewModel Document { get; protected set; }
    }
}
