using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Documents.Views
{
    public class LibraryItemDisplay : ItemDisplay//, IDrawingViewModelItem
    {
        public LibraryItemDisplay()
        {
        }

        public PreviewLibraryItemViewModel Preview { get; set; }

        public ISolutionProjectNodeModel ProjectModel { get; set; }

        public override void OnPreviewChanged()
        {
            OnPropertyChanged(nameof(Preview));
        }

        public override void PreviewDocument()
        {
            var libItem = Document as LibraryItem;
            if (Preview == null)
                Preview = PreviewLibraryItemViewModel.CreateFromDocument(libItem);

            Preview?.PreviewDocument(libItem, ProjectModel);

            OnPropertyChanged(nameof(Preview));
        }
    }
}
