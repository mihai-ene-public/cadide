namespace IDE.Documents.Views
{
    public interface ISaveFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }

        bool? ShowDialog();
    }
}