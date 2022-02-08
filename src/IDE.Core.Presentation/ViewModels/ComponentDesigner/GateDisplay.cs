using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core;

namespace IDE.Documents.Views
{
    public class GateDisplay : BaseViewModel
    {
        public GateDisplay()
        {
            //Preview = new PreviewCanvasItemViewModel
            //{
            //    CanvasModel = new DrawingViewModel(null)
            //};
        }

        public string Name
        {
            get
            {
                return Gate.name;
            }
            set
            {
                Gate.name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName
        {
            get { return $"{Gate.name}: {Symbol.Name} ({Symbol.Library})"; }
        }

        //public IDrawingViewModel Canvas { get; set; }

        public Gate Gate { get; set; }

        public Symbol Symbol { get; set; }

        public PreviewLibraryItemViewModel Preview { get; set; } = new PreviewSymbolViewModel();
    }

}
