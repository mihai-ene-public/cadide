using System.Collections;

namespace IDE.Core.Interfaces
{
    public interface ICanvasDesignerFileViewModel: IFileBaseViewModel, ISnapshotManager
    {
        IDrawingViewModel CanvasModel { get; set; }

        /// <summary>
        /// If item can be selected based on selection filter
        /// </summary>
        bool CanSelectItem(ISelectableItem item);

        IList CanSelectList { get; }
        void MirrorXSelectedItems();

        void MirrorYSelectedItems();

        void ChangeFootprintPlacement();
    }
}
