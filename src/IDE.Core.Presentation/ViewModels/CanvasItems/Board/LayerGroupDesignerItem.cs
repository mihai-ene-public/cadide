using IDE.Core.Interfaces;
using System.Collections.Generic;

namespace IDE.Core.Designers
{
    //there are some predefined groups that cannot pe edited: All Layers, Signal Layers, Plane Layers,...
    //the actual layers are defined on the board or footprint
    //a group is a filtered group of layers
    //groups are stored on a board or footprint
    //for footprints there is a standard list of groups (All)

    /// <summary>
    /// contains a grouped list of layers; a layer can belong to multiple groups
    /// </summary>
    public class LayerGroupDesignerItem : EditBoxModel, ILayerGroupDesignerItem
    {
        public LayerGroupDesignerItem()
        {
            Layers = new SortableObservableCollection<ILayerDesignerItem>();
        }

        bool isReadOnly = true;

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Layers list that belongs to this group
        /// </summary>
        public IList<ILayerDesignerItem> Layers { get; private set; }

        public void LoadLayers(IList<ILayerDesignerItem> layers)
        {
            Layers.Clear();
            Layers.AddRange(layers);
        }

        public void SortLayers()
        {

        }
    }
}
