using IDE.Core.Storage;

namespace IDE.Documents.Views
{
    public class DocumentSizeTemplate
    {
        public DocumentSize DocumentSize { get; set; }
        public double DocumentWidth { get; set; }
        public double DocumentHeight { get; set; }

        public override string ToString()
        {
            return DocumentSize.ToString();
        }
    }
}
