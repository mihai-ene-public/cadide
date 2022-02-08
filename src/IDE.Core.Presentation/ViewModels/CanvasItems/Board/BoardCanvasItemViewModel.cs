using IDE.Core.Interfaces;
using System.ComponentModel;

namespace IDE.Core.Designers
{
    /// <summary>
    /// a designer item for drawing on a board or footprint; it belongs to a layer
    /// </summary>
    public abstract class BoardCanvasItemViewModel : BaseCanvasItem
    {

        public BoardCanvasItemViewModel()
        {
        }

        [Browsable(false)]
        ////not used, because these items are layered
        public override int ZIndex { get; set; }

        /// <summary>
        /// footprint or the board this item belongs to
        /// </summary>
        [Browsable(false)]
        public ILayeredViewModel LayerDocument { get; set; }

        
        private bool isFaulty;

        [Browsable(false)]
        public bool IsFaulty
        {
            get { return isFaulty; }
            set
            {
                isFaulty = value;
                OnPropertyChanged(nameof(IsFaulty));
            }
        }


        #region Layer

        public abstract void LoadLayers();

        protected IItemWithClearance GetItemWithClearance(ISelectableItem item, double clearance)
        {
            //if (item is PadBaseCanvasItem pad)
            //{
            //    pad.Width += 2 * clearance;
            //    pad.Height += 2 * clearance;

            //    if (pad.CornerRadius > 0.0d)
            //    {
            //        pad.CornerRadius += clearance;
            //    }
            //}
            //else if (item is PolygonBoardCanvasItem poly)
            //{
            //    //poly.BorderWidth += 2 * clearance;
            //    //we want to cutout entire polygon
            //    poly.IsFilled = true;
            //}
            //else if (item is TrackBoardCanvasItem track)
            //{
            //    track.Width += 2 * clearance;
            //}
            //else if (item is ViaCanvasItem via)
            //{
            //    via.Diameter += 2 * clearance;
            //}

            return new ItemWithClearance(item, clearance);
        }

        #endregion Layer



    }


}
