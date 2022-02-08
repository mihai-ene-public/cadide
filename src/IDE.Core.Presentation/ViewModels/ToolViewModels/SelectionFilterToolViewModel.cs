using IDE.Core.Interfaces;
using IDE.Documents.Views;
using System.Windows.Input;

namespace IDE.Core.ViewModels
{
    public class SelectionFilterToolViewModel : ToolViewModel, IRegisterable, IDocumentToolWindow
    {
        public SelectionFilterToolViewModel() 
            : base("Selection")
        {
            CanHide = true;
            IsVisible = false;
        }

        ICanvasDesignerFileViewModel document;
        public ICanvasDesignerFileViewModel Document
        {
            get { return document; }
            private set
            {
                document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        //commands: selectAll, selectNone
        ICommand selectAllCommand;
        public ICommand SelectAllCommand
        {
            get
            {
                if (selectAllCommand == null)
                    selectAllCommand = CreateCommand( p =>
                    {
                        foreach (SelectionFilterItemViewModel item in Document.CanSelectList)
                            item.CanSelect = true;
                    });

                return selectAllCommand;
            }
        }

        ICommand selectNoneCommand;
        public ICommand SelectNoneCommand
        {
            get
            {
                if (selectNoneCommand == null)
                    selectNoneCommand = CreateCommand(p =>
                    {
                        foreach (SelectionFilterItemViewModel item in Document.CanSelectList)
                            item.CanSelect = false;
                    });

                return selectNoneCommand;
            }
        }

        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Left;
            }
        }

        public void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {

        }

        public void SetDocument(IFileBaseViewModel document)
        {
            Document = document as ICanvasDesignerFileViewModel;
        }
    }
}
