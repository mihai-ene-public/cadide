using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class ToolBoxViewModel : BaseViewModel
    {

        public ToolBoxViewModel(DrawingViewModel canvas)
        {
            canvasModel = canvas;
        }


        DrawingViewModel canvasModel;
        public ISolutionProjectNodeModel CurrentProject => canvasModel.FileDocument.ProjectNode;

        //ToolBoxItem placedObject;

        ICommand placeObjectCommand;
        public ICommand PlaceObjectCommand
        {
            get
            {
                if (placeObjectCommand == null)
                    placeObjectCommand = CreateCommand((p) =>
                                          {
                                              StartPlacingObject((ToolBoxItem)p);
                                          }
                                       );

                return placeObjectCommand;
            }
        }



        public ObservableCollection<ToolBoxItem> Primitives { get; set; } = new ObservableCollection<ToolBoxItem>();


        void StartPlacingObject(ToolBoxItem tbItem)
        {
            canvasModel.StartPlacement(tbItem.Type, tbItem.PlacementToolType);
        }
    }
}
