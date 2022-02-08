using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    //via, smd, pad
    public abstract class MultiLayerBoardCanvasItem : BoardCanvasItemViewModel
    {

        //in order to determine the layers, it is enough to know if we are on top or bottom

        ILayerDesignerItem copperLayer;

        [Browsable(false)]
        public ILayerDesignerItem CopperLayer
        {
            get { return copperLayer; }
            set
            {
                copperLayer = value;
                OnPropertyChanged(nameof(CopperLayer));
            }
        }

        ILayerDesignerItem pasteLayer;

        [Browsable(false)]
        public ILayerDesignerItem PasteLayer
        {
            get { return pasteLayer; }
            set
            {
                pasteLayer = value;
                OnPropertyChanged(nameof(PasteLayer));
            }
        }

        ILayerDesignerItem solderMaskLayer;

        [Browsable(false)]
        public ILayerDesignerItem SolderMaskLayer
        {
            get { return solderMaskLayer; }
            set
            {
                solderMaskLayer = value;
                OnPropertyChanged(nameof(SolderMaskLayer));
            }
        }

        FootprintPlacement placement = FootprintPlacement.Top;
        [Browsable(false)]
        public FootprintPlacement Placement
        {
            get { return placement; }
            set
            {
                placement = value;
                OnPropertyChanged(nameof(Placement));

                LoadLayers();
            }
        }

        public override void LoadLayers()
        {
            if (LayerDocument == null)
                return;
            var documentLayers = LayerDocument.LayerItems;
            var copperLayerId = Placement == FootprintPlacement.Top ? LayerConstants.SignalTopLayerId : LayerConstants.SignalBottomLayerId;
            var topPasteLayerId = Placement == FootprintPlacement.Top ? LayerConstants.PasteTopLayerId : LayerConstants.PasteBottomLayerId;
            var topSolderMaskLayerId = Placement == FootprintPlacement.Top ? LayerConstants.SolderTopLayerId : LayerConstants.SolderBottomLayerId;

            //copper layer
            var cLayer = documentLayers.FirstOrDefault(l => l.LayerId == copperLayerId);
            if (cLayer != null)
            {
                // cLayer.HideLayerWhenDimmed = true;
                CopperLayer = cLayer;
            }

            //top paste
            var tLayer = documentLayers.FirstOrDefault(l => l.LayerId == topPasteLayerId);
            if (tLayer != null)
            {
                // tLayer.HideLayerWhenDimmed = true;
                PasteLayer = tLayer;
            }
            //top solder mask
            var sLayer = documentLayers.FirstOrDefault(l => l.LayerId == topSolderMaskLayerId);
            if (sLayer != null)
            {
                // sLayer.HideLayerWhenDimmed = true;
                SolderMaskLayer = sLayer;
            }

        }
    }
}
