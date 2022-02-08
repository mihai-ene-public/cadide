using System.Collections.ObjectModel;

namespace IDE.Core.Toolbars
{
    public static class ToolbarManager// : BaseViewModel
    {
        static ToolbarManager()
        {
            Toolbars = new ObservableCollection<ToolbarModel>();
            Toolbars.Add(new GeneralToolbar());
            //Toolbars.Add(new FootprintToolbar());
            //Toolbars.Add(new ModelToolbar());
        }
        public static ObservableCollection<ToolbarModel> Toolbars { get; set; }
    }
}
