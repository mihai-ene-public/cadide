using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Types.Media;


namespace IDE.Core.Designers
{
    public class BusSegmentCanvasItem : BaseCanvasItem
    {
        public BusSegmentCanvasItem()
        {
            PropertyChanged += BusSegmentCanvasItem_PropertyChanged;
        }

        private void BusSegmentCanvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSelected))
            {
                HighlightOwnedBus(IsSelected);
            }
        }

        public void HighlightOwnedBus(bool newHighlight)
        {
            if (bus == null || bus.IsHighlighted == newHighlight)
                return;

            bus.HighlightBus(newHighlight);
        }

        SchematicBus bus;
        /// <summary>
        /// reference to the net this object belongs to
        /// </summary>
        [Browsable(false)]
        public virtual SchematicBus Bus
        {
            get
            {
                return bus;
            }
            set
            {
                if (bus != null)
                {
                    bus.PropertyChanged -= Bus_PropertyChanged;

                    //remove from the old net this item
                    bus.BusItems.Remove(this);
                }

                bus = value;


                if (bus != null)
                {
                    if (!bus.BusItems.Contains(this))
                        bus.BusItems.Add(this);
                    bus.PropertyChanged += Bus_PropertyChanged;
                }

                OnPropertyChanged(nameof(Bus));
            }
        }

        public void AssignBus(SchematicBus newBus)
        {
            bus = newBus;
        }

        void Bus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Bus.Name");
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

        public override void Rotate()
        {
            throw new NotImplementedException("should inherit");
        }

        public override void RemoveFromCanvas()
        {
            base.RemoveFromCanvas();

            Bus = null;
        }
    }
}
