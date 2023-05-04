using System.Collections;
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Messages;
using IDE.Core.Presentation.Placement;
using IDE.Documents.Views;

namespace IDE.Core.ViewModels;

public class PropertiesToolWindowViewModel : ToolViewModel, IPropertiesToolWindow
{
    public PropertiesToolWindowViewModel()
        : base("Properties")
    {


        Messenger.Register<IPropertiesToolWindow, SolutionClosedMessage>(this,
            (vm, message) =>
            {
                IsVisible = false;
            });

        Messenger.Register<IPropertiesToolWindow, CanvasSelectionChangedMessage>(this,
                    (vm, message) => CanvasSelectionChangedHandler(message));
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

    private void CanvasSelectionChangedHandler(CanvasSelectionChangedMessage message)
    {
        var oldSelected = SelectedObject;
        SelectedObject = null;

        var canvasModel = message.Sender as ICanvasDesignerFileViewModel;
        if (canvasModel == null)
        {
            return;
        }

        if (canvasModel.PlacementTool != null)
        {
            SelectedObject = canvasModel.PlacementTool.CanvasItem;
        }
        else
        {
            var items = canvasModel.SelectedItems;

            if (items.Count == 1)
            {
                SelectedObject = items.FirstOrDefault();
            }
            else if (items.Count > 1)
            {
                SelectedObject = new MultipleSelectionObject()
                {
                    SelectedObjects = (IList)items,
                };
            }
        }

        //show properties to view (this could be an option)
        if (SelectedObject != null)
        {
            IsVisible = true;

            if (oldSelected != SelectedObject)
                IsActive = true;
        }
    }
}
