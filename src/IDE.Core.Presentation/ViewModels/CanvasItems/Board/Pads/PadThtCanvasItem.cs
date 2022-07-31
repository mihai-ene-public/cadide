using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class PadThtCanvasItem : PadSimpleCanvasItem, IPadThtCanvasItem
    {
        public PadThtCanvasItem()
            : base()
        {
        }

        RectangleBoardCanvasItem topCopperItem;
        RectangleBoardCanvasItem topPasteItem;
        RectangleBoardCanvasItem topSolderMaskItem;

        RectangleBoardCanvasItem botCopperItem;
        RectangleBoardCanvasItem botPasteItem;
        RectangleBoardCanvasItem botSolderMaskItem;

        HoleCanvasItem holeCanvasitem;

        ILayerDesignerItem topCopperLayer;

        [Browsable(false)]
        public ILayerDesignerItem TopCopperLayer
        {
            get
            {
                return topCopperLayer;
            }
            set
            {
                if (topCopperLayer != value)
                {
                    topCopperLayer = value;
                    OnPropertyChanged(nameof(TopCopperLayer));
                }
            }
        }

        ILayerDesignerItem topPasteLayer;

        [Browsable(false)]
        public ILayerDesignerItem TopPasteLayer
        {
            get
            {
                return topPasteLayer;
            }
            set
            {
                if (topPasteLayer != value)
                {
                    topPasteLayer = value;
                    OnPropertyChanged(nameof(TopPasteLayer));
                }
            }
        }

        ILayerDesignerItem topSolderMaskLayer;

        [Browsable(false)]
        public ILayerDesignerItem TopSolderMaskLayer
        {
            get
            {
                return topSolderMaskLayer;
            }
            set
            {
                if (topSolderMaskLayer != value)
                {
                    topSolderMaskLayer = value;
                    OnPropertyChanged(nameof(TopSolderMaskLayer));
                }
            }
        }

        ILayerDesignerItem bottomCopperLayer;

        [Browsable(false)]
        public ILayerDesignerItem BottomCopperLayer
        {
            get
            {
                return bottomCopperLayer;
            }
            set
            {
                if (bottomCopperLayer != value)
                {
                    bottomCopperLayer = value;
                    OnPropertyChanged(nameof(BottomCopperLayer));
                }
            }
        }

        ILayerDesignerItem bottomPasteLayer;

        [Browsable(false)]
        public ILayerDesignerItem BottomPasteLayer
        {
            get
            {
                return bottomPasteLayer;
            }
            set
            {
                if (bottomPasteLayer != value)
                {
                    bottomPasteLayer = value;
                    OnPropertyChanged(nameof(BottomPasteLayer));
                }
            }
        }

        ILayerDesignerItem bottomSolderMaskLayer;

        [Browsable(false)]
        public ILayerDesignerItem BottomSolderMaskLayer
        {
            get
            {
                return bottomSolderMaskLayer;
            }
            set
            {
                if (bottomSolderMaskLayer != value)
                {
                    bottomSolderMaskLayer = value;
                    OnPropertyChanged(nameof(BottomSolderMaskLayer));
                }
            }
        }


        [Browsable(false)]
        public double Drill => Hole.Drill;

        [Display(Order = 2000)]
        [DisplayName("Hole definition")]
        [Editor(EditorNames.HoleDefinitionEditor, EditorNames.HoleDefinitionEditor)]
        public HoleCanvasItem Hole
        {
            get { return holeCanvasitem; }
            set { }
        }

        [Browsable(false)]
        public XPoint Center
        {
            get
            {
                //same as x,y
                return new XPoint();
            }
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Pad)primitive;

            Items.Clear();

            X = t.x;
            Y = t.y;
            Rot = t.rot;
            width = t.Width;
            height = t.Height;
            cornerRadius = t.CornerRadius;
            Number = t.number;
            AdjustPasteMaskInRules = t.AdjustPasteMaskInRules;
            AutoGeneratePasteMask = t.AutoGeneratePasteMask;
            AdjustSolderMaskInRules = t.AdjustSolderMaskInRules;
            AutoGenerateSolderMask = t.AutoGenerateSolderMask;
            LayerId = t.layerId;//LayerConstants.SignalTopLayerId;

            EnsurePrimitives();

            Hole.Drill = t.drill;
            Hole.DrillType = t.DrillType;
            Hole.X = t.HoleOffsetX;
            Hole.Y = t.HoleOffsetY;
            Hole.Height = t.SlotHeight;
            Hole.Rot = t.SlotRotation;

        }

        void AddItems()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
            }
            // return;

            //if (LayerConstants.IsTopLayer(LayerId))
            //{
            Items.Add(botPasteItem);
            Items.Add(botSolderMaskItem);
            Items.Add(botCopperItem);

            Items.Add(topPasteItem);
            Items.Add(topSolderMaskItem);
            Items.Add(topCopperItem);
            //}
            //else
            //{
            //    Items.Add(topPasteItem);
            //    Items.Add(topSolderMaskItem);
            //    Items.Add(topCopperItem);

            //    Items.Add(botPasteItem);
            //    Items.Add(botSolderMaskItem);
            //    Items.Add(botCopperItem);
            //}


            Items.Add(holeCanvasitem);
        }

        //void AddTopItems()
        //{

        //}

        protected override void EnsurePrimitives()
        {
            EnsurePasteItems();
            EnsureSolderMaskItems();
            EnsureCopperItems();
            EnsureDrills();

            AddItems();

            SetLayers();
        }

        void SetLayers()
        {
            TopCopperLayer = topCopperItem?.Layer;
            TopPasteLayer = topPasteItem?.Layer;
            TopSolderMaskLayer = topSolderMaskItem?.Layer;

            BottomCopperLayer = botCopperItem?.Layer;
            BottomPasteLayer = botPasteItem?.Layer;
            BottomSolderMaskLayer = botSolderMaskItem?.Layer;
        }

        private void EnsureCopperItems()
        {
            if (topCopperItem == null)
                topCopperItem = new RectangleBoardCanvasItem();
            if (botCopperItem == null)
                botCopperItem = new RectangleBoardCanvasItem();

            topCopperItem.ParentObject = this;
            topCopperItem.BorderWidth = 0;
            topCopperItem.CornerRadius = CornerRadius;
            topCopperItem.Width = Width;
            topCopperItem.Height = Height;
            topCopperItem.IsFilled = true;
            topCopperItem.AssignLayer(Layer);

            botCopperItem.ParentObject = this;
            botCopperItem.BorderWidth = 0;
            botCopperItem.CornerRadius = CornerRadius;
            botCopperItem.Width = Width;
            botCopperItem.Height = Height;
            botCopperItem.IsFilled = true;
            var botLayerId = LayerConstants.GetPairedLayer(LayerId, GetBottomPlacement());
            botCopperItem.AssignLayer(GetLayer(botLayerId));
        }

        FootprintPlacement GetBottomPlacement()
        {
            var placement = FootprintPlacement.Top;

            if (ParentObject is FootprintBoardCanvasItem fp)
            {
                placement = fp.Placement;
            }

            //toggle from bottom to top or from top to bototm
            placement = (FootprintPlacement)(1 - (int)placement);

            return placement;
        }

        private void EnsureSolderMaskItems()
        {
            if (topSolderMaskItem == null)
                topSolderMaskItem = new RectangleBoardCanvasItem();
            if (botSolderMaskItem == null)
                botSolderMaskItem = new RectangleBoardCanvasItem();

            topSolderMaskItem.ParentObject = this;
            topSolderMaskItem.BorderWidth = 0;
            topSolderMaskItem.CornerRadius = CornerRadius;
            topSolderMaskItem.Width = Width;
            topSolderMaskItem.Height = Height;
            topSolderMaskItem.IsFilled = true;
            var topSolderMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(LayerId, LayerType.SolderMask));
            topSolderMaskItem.AssignLayer(topSolderMaskLayer);

            botSolderMaskItem.ParentObject = this;
            botSolderMaskItem.BorderWidth = 0;
            botSolderMaskItem.CornerRadius = CornerRadius;
            botSolderMaskItem.Width = Width;
            botSolderMaskItem.Height = Height;
            botSolderMaskItem.IsFilled = true;
            var botLayerId = LayerConstants.GetPairedLayer(LayerId, GetBottomPlacement());
            var botSolderMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(botLayerId, LayerType.SolderMask));
            botSolderMaskItem.AssignLayer(botSolderMaskLayer);
        }

        private void EnsurePasteItems()
        {
            if (topPasteItem == null)
                topPasteItem = new RectangleBoardCanvasItem();
            if (botPasteItem == null)
                botPasteItem = new RectangleBoardCanvasItem();

            topPasteItem.ParentObject = this;
            topPasteItem.BorderWidth = 0;
            topPasteItem.CornerRadius = CornerRadius;
            topPasteItem.Width = Width;
            topPasteItem.Height = Height;
            topPasteItem.IsFilled = true;
            var topPasteMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(LayerId, LayerType.PasteMask));
            topPasteItem.AssignLayer(topPasteMaskLayer);

            botPasteItem.ParentObject = this;
            botPasteItem.BorderWidth = 0;
            botPasteItem.CornerRadius = CornerRadius;
            botPasteItem.Width = Width;
            botPasteItem.Height = Height;
            botPasteItem.IsFilled = true;
            var botLayerId = LayerConstants.GetPairedLayer(LayerId, GetBottomPlacement());
            var botPasteMaskLayer = GetLayer(LayerConstants.GetCompanionLayer(botLayerId, LayerType.PasteMask));
            botPasteItem.AssignLayer(botPasteMaskLayer);
        }

        void EnsureDrills()
        {
            if (holeCanvasitem == null)
                holeCanvasitem = new HoleCanvasItem();
        }

        protected override void PropertyChangedHandle(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Layer):
                case nameof(Width):
                case nameof(Height):
                case nameof(CornerRadius):
                    //case nameof(Drill):
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
            var t = new Pad();

            t.drill = Hole.Drill;
            t.DrillType = Hole.DrillType;
            t.HoleOffsetX = Hole.X;
            t.HoleOffsetY = Hole.Y;
            t.SlotHeight = Hole.Height;
            t.SlotRotation = Hole.Rot;
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

            return $"Pad ({Number}{s})";
        }
    }

}
