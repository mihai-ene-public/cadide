using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views;

public interface IPreviewCanvasViewModel: ICanvasViewModel
{

    /// <summary>
    /// Origin in top-left coordinates
    /// </summary>
    XPoint Origin { get; set; }

    double DocumentWidth { get; set; }
    double DocumentHeight { get; set; }

    XColor GridColor { get; set; }
    XColor CanvasBackground { get; set; }

    IList<ISelectableItem> Items { get; set; }
    void ZoomToFit();

}
