namespace IDE.Documents.Views
{
    public interface IOpenFileDialog
    {
        string DefaultExt { get; set; }
        string FileName { get; set; }
        string Filter { get; set; }

        string InitialDirectory { get; set; }
        bool Multiselect { get; set; }
        int FilterIndex { get; set; }

        string[] FileNames { get; }

        bool? ShowDialog();
    }
}