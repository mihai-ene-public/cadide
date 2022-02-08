namespace IDE.Documents.Views
{
    public class ComponentItemDisplay : ItemDisplay
    {
        public LibraryItemDisplay Symbol { get; set; }

        public LibraryItemDisplay Footprint { get; set; }

        public override void OnPreviewChanged()
        {
            Symbol?.OnPropertyChanged(nameof(Symbol.Preview));
            Footprint?.OnPropertyChanged(nameof(Symbol.Preview));
        }

        public override void PreviewDocument()
        {
            Symbol?.PreviewDocument();
            Footprint?.PreviewDocument();
        }
    }
}
