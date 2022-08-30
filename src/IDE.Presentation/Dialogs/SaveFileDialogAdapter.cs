using Microsoft.Win32;

namespace IDE.Documents.Views;

public class SaveFileDialogAdapter : ISaveFileDialog
{

    public SaveFileDialogAdapter()
    {
        dlg = new SaveFileDialog();
    }

    SaveFileDialog dlg = null;

    public string FileName
    {
        get { return dlg.FileName; }
        set { dlg.FileName = value; }
    }
    public string Filter
    {
        get { return dlg.Filter; }
        set { dlg.Filter = value; }
    }
    public string InitialDirectory
    {
        get { return dlg.InitialDirectory; }
        set { dlg.InitialDirectory = value; }
    }

    public bool? ShowDialog()
    {
        return dlg.ShowDialog();
    }
}
