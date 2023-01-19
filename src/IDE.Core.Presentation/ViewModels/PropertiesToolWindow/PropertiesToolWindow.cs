using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Messages;
using IDE.Documents.Views;

namespace IDE.Core.ViewModels
{
    public class PropertiesToolWindowViewModel : ToolViewModel, IPropertiesToolWindow
    {
        public PropertiesToolWindowViewModel()
            : base("Properties")
        {


            StrongReferenceMessenger.Default.Register<IPropertiesToolWindow, SolutionClosedMessage>(this,
                (vm, message) =>
                {
                    IsVisible = false;
                });
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
