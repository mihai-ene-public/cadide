using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class PropertiesToolWindowViewModel : ToolViewModel, IPropertiesToolWindow
    {
        public PropertiesToolWindowViewModel()
            : base("Properties")
        {

        }

        object selectedObject;

        public object SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }

        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Right;
            }
        }

    }

}
