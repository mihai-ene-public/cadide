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

        public ToolBoxViewModel(ICanvasDesignerFileViewModel canvasFileDocument)
        {
            canvasModel = canvasFileDocument;
        }


        private readonly ICanvasDesignerFileViewModel canvasModel;

        ICommand placeObjectCommand;
        public ICommand PlaceObjectCommand
        {
            get
            {
                if (placeObjectCommand == null)
                    placeObjectCommand = CreateCommand((p) =>
                                          {
                                              StartPlacingObject((IToolboxItem)p);
                                          }
                                       );

                return placeObjectCommand;
            }
        }



        public ObservableCollection<ToolBoxItem> Primitives { get; set; } = new ObservableCollection<ToolBoxItem>();


        private void StartPlacingObject(IToolboxItem toolBoxItem)
        {
            canvasModel.StartPlacement(toolBoxItem);
        }
    }
}
