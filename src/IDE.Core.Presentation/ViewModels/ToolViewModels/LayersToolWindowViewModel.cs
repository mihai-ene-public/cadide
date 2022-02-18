using System.Collections.Generic;
using System.Linq;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class LayersToolWindowViewModel : ToolViewModel, ILayersToolWindow
    {
        public LayersToolWindowViewModel()
            : base("Layers")
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

        ILayeredViewModel layeredDocument;
        public ILayeredViewModel LayeredDocument
        {
            get
            {
                return layeredDocument;
            }
            private set
            {
                if (layeredDocument == value)
                    return;

                layeredDocument = value;


                if (layeredDocument != null)
                    layeredDocument.SelectedLayerGroup = ((IList<LayerGroupDesignerItem>)layeredDocument.LayerGroups).FirstOrDefault();

                OnPropertyChanged(nameof(LayeredDocument));
            }
        }

        public void SetDocument(IFileBaseViewModel document)
        {
            LayeredDocument = document as ILayeredViewModel;
        }
    }
}
