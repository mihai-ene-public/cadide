using System.Collections.Generic;
using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces;

public interface ICanvasViewModel
{
    double X { get; set; }
    double Y { get; set; }

    XPoint Offset { get; set; }

    double Scale { get; set; }


    double GridSize { get; }

    ICanvasGrid CanvasGrid { get; }

    List<ISelectableItem> SelectedItems { get; }

    IEnumerable<ISelectableItem> GetItems();

    void UpdateSelection();
    void ClearSelectedItems();
}
