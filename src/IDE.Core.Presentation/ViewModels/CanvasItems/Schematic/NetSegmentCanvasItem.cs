using IDE.Core.Types.Media;
using System;
using System.ComponentModel;

namespace IDE.Core.Designers
{
    /// <summary>
    /// base item belonging to a net
    /// </summary>
    public class NetSegmentCanvasItem : BaseCanvasItem
    {
        public NetSegmentCanvasItem()
        {
            //PropertyChanged += NetSegmentDesignerItem_PropertyChanged;
        }

        //private void NetSegmentDesignerItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(IsSelected))
        //    {
        //        HighlightOwnedNet(IsSelected);
        //    }
        //}

        protected override void SetIsSelectedInternal(bool value)
        {
            base.SetIsSelectedInternal(value);
            HighlightOwnedNet(value);
        }

        public void HighlightOwnedNet(bool newHighlight)
        {
            if (net == null || net.IsHighlighted == newHighlight)
                return;

            net.IsHighlighted = newHighlight;
            
            //todo: last cross highlight
            /*
            var canvasModel = net.CanvasModel;

            canvasModel.OnHighlightChanged(canvasModel, EventArgs.Empty);
            */
        }

        SchematicNet net;
        /// <summary>
        /// reference to the net this object belongs to
        /// </summary>
        [Browsable(false)]
        public virtual SchematicNet Net
        {
            get
            {
                return net;
            }
            set
            {
                if (net != null)
                {
                    net.PropertyChanged -= Net_PropertyChanged;

                    //remove from the old net this item
                    net.NetItems.Remove(this);
                }

                net = value;


                if (net != null)
                {
                    if (!net.NetItems.Contains(this))
                        net.NetItems.Add(this);
                    net.PropertyChanged += Net_PropertyChanged;
                }

                OnPropertyChanged(nameof(Net));
            }
        }

        public void AssignNet(SchematicNet newNet)
        {
            net = newNet;
        }

        void Net_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Net.Name");
        }



        public override XRect GetBoundingRectangle()
        {
            throw new NotImplementedException("should inherit");
        }

        public override void Translate(double dx, double dy)
        {
            throw new NotImplementedException("should inherit");
        }

        public override void MirrorX()
        {
            throw new NotImplementedException("should inherit");
        }

        public override void MirrorY()
        {
            throw new NotImplementedException("should inherit");
        }

        public override void Rotate(double angle = 90)
        {
            throw new NotImplementedException("should inherit");
        }

        public override void RemoveFromCanvas()
        {
            base.RemoveFromCanvas();

            Net = null;
        }
    }
}
