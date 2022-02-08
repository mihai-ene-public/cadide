using System.Collections.ObjectModel;
using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core;

namespace IDE.Documents.Views
{
    public class FootprintDisplay : BaseViewModel//, IDrawingViewModelItem
    {
        public FootprintDisplay()
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
                return $"{Footprint.Name} ({Footprint.Library})";
            }
            set
            {
                Footprint.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public PreviewLibraryItemViewModel Preview { get; set; } = new PreviewFootprintViewModel();

        public Footprint Footprint { get; set; }

        public FootprintRef Device { get; set; }

        public ObservableCollection<ConnectDisplay> Connects { get; set; }
    }

}
