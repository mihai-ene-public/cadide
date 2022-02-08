using IDE.Core.Interfaces;
using IDE.Presentation.Dialogs.FolderBrowserDialog;

// ------------------------------------------------------------------
// Wraps System.Windows.Forms.OpenFileDialog to make it present
// a vista-style dialog.
// ------------------------------------------------------------------

namespace IDE.Documents.Views
{
    public class FolderSelectDialogAdapter : IFolderSelectDialog
    {
        public FolderSelectDialogAdapter()
        {
            dlg = new VistaFolderBrowserDialog();
            dlg.Description = "Select a folder";
            dlg.UseDescriptionForTitle = true;
        }

        VistaFolderBrowserDialog dlg;

        public string Title
        {
            get { return dlg.Description; }
            set { dlg.Description = value; }
        }
        public string FileName
        {
            get
            {
                return dlg.SelectedPath;
            }
        }
        public string InitialDirectory {
            get
            {
                return dlg.SelectedPath;
            }
            set
            {
                dlg.SelectedPath = value;
            }
        }

        public bool ShowDialog()
        {
            return dlg.ShowDialog() == true;
        }
    }
}
