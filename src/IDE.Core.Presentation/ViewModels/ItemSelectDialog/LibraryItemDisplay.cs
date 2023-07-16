using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Documents.Views;

public class LibraryItemDisplay : ItemDisplay
{


    public LibraryItemDisplay(ProjectInfo project)
    {
        _project = project;
    }

    private readonly ProjectInfo _project;
    public PreviewLibraryItemViewModel Preview { get; set; }

    public override void OnPreviewChanged()
    {
        OnPropertyChanged(nameof(Preview));
    }

    public override void PreviewDocument()
    {
        var libItem = Document as LibraryItem;
        if (libItem == null)
        {
            return;
        }

        if (Preview == null)
        {
            Preview = PreviewLibraryItemViewModel.CreateFromDocument(libItem);
            Preview.SetProject(_project);
        }

        Preview?.PreviewDocument(libItem);

        OnPropertyChanged(nameof(Preview));
    }
}
