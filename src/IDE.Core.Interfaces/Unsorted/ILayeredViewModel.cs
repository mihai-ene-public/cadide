using IDE.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{

    /// <summary>
    /// an object view model that has layers: (footprint, board, [gerber])
    /// </summary>
    public interface ILayeredViewModel
    {
        IList LayerGroups { get; }

        IList<ILayerDesignerItem> LayerItems { get; }

        ILayerDesignerItem SelectedLayer { get; set; }

        ILayerGroupDesignerItem SelectedLayerGroup { get; set; }

        bool MaskUnselectedLayer { get; set; }

        bool HideUnselectedLayer { get; set; }
    }

   

  
}
