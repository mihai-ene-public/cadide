
using Microsoft.Win32;

namespace IDE.Documents.Views
{
    public class OpenFileDialogAdapter : IOpenFileDialog
    {
        public OpenFileDialogAdapter()
        {
            dlg = new OpenFileDialog();
        }

        OpenFileDialog dlg = null;

        public string Filter
        {
            get { return dlg.Filter; }
            set { dlg.Filter = value; }
        }

        public string DefaultExt
        {
            get { return dlg.DefaultExt; }
            set { dlg.DefaultExt = value; }
        }

        public string FileName
        {
            get { return dlg.FileName; }
            set { dlg.FileName = value; }
        }

        public string[] FileNames
        {
            get { return dlg.FileNames; }
        }

        public bool Multiselect
        {
            get { return dlg.Multiselect; }
            set { dlg.Multiselect = value; }
        }

        public string InitialDirectory
        {
            get { return dlg.InitialDirectory; }
            set { dlg.InitialDirectory = value; }
        }
        public int FilterIndex
        {
            get { return dlg.FilterIndex; }
            set { dlg.FilterIndex = value; }
        }

        public bool? ShowDialog()
        {
            return dlg.ShowDialog();
        }
    }
}
