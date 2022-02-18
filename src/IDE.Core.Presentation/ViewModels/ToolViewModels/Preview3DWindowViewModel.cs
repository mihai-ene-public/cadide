using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class Preview3DWindowViewModel : ToolViewModel, IPreview3DToolWindow
    {
        public Preview3DWindowViewModel()
          : base("Preview options")
        {
            CanHide = true;
            IsVisible = false;

        }

        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Left;
            }
        }

        ILayeredViewModel document;
        public ILayeredViewModel Document
        {
            get { return document; }
            private set
            {
                document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        public void SetDocument(IFileBaseViewModel document)
        {
            Document = document as ILayeredViewModel;
        }
    }
}
