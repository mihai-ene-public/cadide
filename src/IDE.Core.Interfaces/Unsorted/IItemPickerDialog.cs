using System.Collections;

namespace IDE.Core.Interfaces
{
    public interface IItemPickerDialog : IWindow
    {
        void LoadData(IList items);

        object SelectedItem { get; set; }
    }

    public interface IFolderSelectDialog
    {
        string Title { get; set; }

        string FileName { get; }

        string InitialDirectory { get; set; }

        bool ShowDialog();
    }
}
