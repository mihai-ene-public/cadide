using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class SchematicSheetsViewModel : ToolViewModel, ISchematicSheetsToolWindow
    {

        public SchematicSheetsViewModel()
            : base("Sheets")
        {
            CanHide = true;
            IsVisible = false;
        }

        IFileBaseViewModel schematic;
        public //SchematicDesignerViewModel
            IFileBaseViewModel Schematic
        {
            get { return schematic; }
            private set
            {
                schematic = value;
                OnPropertyChanged(nameof(Schematic));
            }
        }

        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Left;
            }
        }

        public void SetDocument(IFileBaseViewModel document)
        {
            Schematic = document;
        }
    }
}
