using IDE.Core;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views
{
    public class GenericLayeredViewModel : BaseViewModel, ILayeredViewModel
    {
        public bool MaskUnselectedLayer
        {
            get; set;
        }

        public bool HideUnselectedLayer
        {
            get; set;
        }

        public IList LayerGroups
        {
            get; set;
        }

        public IList<ILayerDesignerItem> LayerItems
        {
            get; set;
        } = new ObservableCollection<ILayerDesignerItem>();

        public ILayerDesignerItem SelectedLayer
        {
            get; set;
        }

        public ILayerGroupDesignerItem SelectedLayerGroup
        {
            get; set;
        }
        public IDrawingViewModel CanvasModel { get; set; }
    }
}
