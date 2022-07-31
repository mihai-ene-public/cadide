using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IDE.Core.Designers
{
    public class PadSmdCanvasItem : PadSimpleCanvasItem, IPadSmdCanvasItem
    {
        public PadSmdCanvasItem()
            : base()
        {

        }

        RectangleBoardCanvasItem copperItem;
        RectangleBoardCanvasItem pasteItem;
        RectangleBoardCanvasItem solderMaskItem;

        ILayerDesignerItem copperLayer;

        [Browsable(false)]
        public ILayerDesignerItem CopperLayer
        {
            get
            {
                return copperLayer;
            }
            set
            {
                if (copperLayer != value)
                {
                    copperLayer = value;
                    OnPropertyChanged(nameof(CopperLayer));
                }
            }
        }

        ILayerDesignerItem pasteLayer;

        [Browsable(false)]
        public ILayerDesignerItem PasteLayer
        {
            get
            {
                return pasteLayer;
            }
            set
            {
                if (pasteLayer != value)
                {
                    pasteLayer = value;
                    OnPropertyChanged(nameof(PasteLayer));
                }
            }
        }

        ILayerDesignerItem solderMaskLayer;

        [Browsable(false)]
        public ILayerDesignerItem SolderMaskLayer
        {
            get
            {
                return solderMaskLayer;
            }
            set
            {
                if (solderMaskLayer != value)
                {
                    solderMaskLayer = value;
                    OnPropertyChanged(nameof(SolderMaskLayer));
                }
            }
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Smd)primitive;

            Items.Clear();

            X = t.x;
            Y = t.y;
            Rot = t.rot;
            Width = t.Width;
            Height = t.Height;
            CornerRadius = t.CornerRadius;
            Number = t.number;
            AdjustPasteMaskInRules = t.AdjustPasteMaskInRules;
            AutoGeneratePasteMask = t.AutoGeneratePasteMask;
            AdjustSolderMaskInRules = t.AdjustSolderMaskInRules;
            AutoGenerateSolderMask = t.AutoGenerateSolderMask;
            LayerId = t.layerId;

            EnsurePrimitives();
        }

        void AddItems()
        {
            Items.Clear();

            if (pasteItem != null)
                Items.Add(pasteItem);

            if (solderMaskItem != null)
                Items.Add(solderMaskItem);

            if (copperItem != null)
                Items.Add(copperItem);

        }

        protected override void EnsurePrimitives()
        {
            if (AutoGeneratePasteMask)
                EnsurePasteItems();
            else
            {
                pasteItem = null;
            }

            if (AutoGenerateSolderMask)
                EnsureSolderMaskItems();
            else
            {
                solderMaskItem = null;
            }

            EnsureCopperItems();

            AddItems();

            SetLayers();

            AttachLayerChanged();
        }

        void SetLayers()
        {
            CopperLayer = copperItem?.Layer;
            PasteLayer = pasteItem?.Layer;
            SolderMaskLayer = solderMaskItem?.Layer;
        }

        [Browsable(false)]
        public override int ZIndex
        {
            get
            {
                return GetZIndex() + 1;
            }
            set
            {
                if (zIndex != value)
                {

                    zIndex = value;
                    OnPropertyChanged(nameof(ZIndex));
                }
            }
        }

        void AttachLayerChanged()
        {
            if (copperLayer != null)
            {
                copperLayer.PropertyChanged -= Layer_PropertyChanged;
                copperLayer.PropertyChanged += Layer_PropertyChanged;
            }

            if (pasteLayer != null)
            {
                pasteLayer.PropertyChanged -= Layer_PropertyChanged;
                pasteLayer.PropertyChanged += Layer_PropertyChanged;
            }

            if (solderMaskLayer != null)
            {
                solderMaskLayer.PropertyChanged -= Layer_PropertyChanged;
                solderMaskLayer.PropertyChanged += Layer_PropertyChanged;
            }
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayerDesignerItem.ZIndex))
            {
                OnPropertyChanged(nameof(ZIndex));
            }
        }

        private void EnsureCopperItems()
        {
            if (copperItem == null)
                copperItem = new RectangleBoardCanvasItem();

            copperItem.ParentObject = this;
            copperItem.BorderWidth = 0;
            copperItem.CornerRadius = CornerRadius;
            copperItem.Width = Width;
            copperItem.Height = Height;
            copperItem.IsFilled = true;
            copperItem.AssignLayer(Layer);
        }

        private void EnsureSolderMaskItems()
        {
            if (solderMaskItem == null)
                solderMaskItem = new RectangleBoardCanvasItem();

            solderMaskItem.ParentObject = this;
            solderMaskItem.BorderWidth = 0;
            solderMaskItem.CornerRadius = CornerRadius;
            solderMaskItem.Width = Width;
            solderMaskItem.Height = Height;
            solderMaskItem.IsFilled = true;
            var solderMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(LayerId, LayerType.SolderMask));
            solderMaskItem.AssignLayer(solderMaskLayer);
        }

        private void EnsurePasteItems()
        {
            if (pasteItem == null)
                pasteItem = new RectangleBoardCanvasItem();

            pasteItem.ParentObject = this;
            pasteItem.BorderWidth = 0;
            pasteItem.CornerRadius = CornerRadius;
            pasteItem.Width = Width;
            pasteItem.Height = Height;
            pasteItem.IsFilled = true;
            var pasteMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(LayerId, LayerType.PasteMask));
            pasteItem.AssignLayer(pasteMaskLayer);
        }

        protected override void PropertyChangedHandle(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Layer):
                case nameof(Width):
                case nameof(Height):
                case nameof(CornerRadius):
                case nameof(AutoGenerateSolderMask):
                case nameof(AutoGeneratePasteMask):
                    EnsurePrimitives();
                    break;
            }

            switch (propertyName)
            {
                case nameof(Width):
                case nameof(Height):
                case nameof(Number):
                case nameof(Signal):
                    OnPropertyChanged(nameof(PadNameFontSize));
                    OnPropertyChanged(nameof(PadNamePosition));
                    OnPropertyChanged(nameof(SignalNameFontSize));
                    break;

            }

            switch (propertyName)
            {
                case nameof(Layer):
                    OnPropertyChanged(nameof(PadTextZIndex));
                    break;
            }
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new Smd();

            t.x = X;
            t.y = Y;
            t.rot = Rot;
            t.Width = Width;
            t.Height = Height;
            t.CornerRadius = CornerRadius;
            t.number = Number;
            t.AdjustPasteMaskInRules = AdjustPasteMaskInRules;
            t.AutoGeneratePasteMask = AutoGeneratePasteMask;
            t.AdjustSolderMaskInRules = AdjustSolderMaskInRules;
            t.AutoGenerateSolderMask = AutoGenerateSolderMask;
            t.layerId = LayerId;

            return t;
        }

        public override string ToString()
        {
            var s = string.Empty;
            if (Signal != null)
                s = $" - {Signal}";

            return $"Smd Pad ({Number}{s})";
        }
    }

}
