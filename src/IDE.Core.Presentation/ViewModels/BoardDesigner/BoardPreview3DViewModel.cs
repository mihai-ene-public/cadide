using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using IDE.Core.Designers;

namespace IDE.Documents.Views
{
    public class BoardPreview3DViewModel : BaseViewModel
    {
        public BoardPreview3DViewModel(ICanvasDesignerFileViewModel canvasDesigner, IDispatcherHelper dispatcher)
        {
            Model3DCanvasModel = new DrawingViewModel(canvasDesigner, dispatcher);
            Model3DCanvasModel.DrawingChanged += Model3DCanvasModel_DrawingChanged;
        }

        #region Model3DCanvasModel

        protected IDrawingViewModel model3DCanvasModel;
        public IDrawingViewModel Model3DCanvasModel
        {
            get
            {
                return model3DCanvasModel;
            }
            set
            {
                if (model3DCanvasModel != value)
                {
                    model3DCanvasModel = value;
                    OnPropertyChanged(nameof(Model3DCanvasModel));
                }
            }
        }

        void Model3DCanvasModel_DrawingChanged(DrawingChangedReason reason)
        {
            //IsDirty = true;
        }



        #endregion Model3DCanvasModel

        public IList<Preview3DLayerData> Preview3DLayers { get; set; } = new List<Preview3DLayerData>();

        // public IList<ISelectableItem> Items { get; set; } = new List<ISelectableItem>();
        public IList<ISelectableItem> Items
        {
            get
            {
                return model3DCanvasModel?.Items;
            }
            set
            {
                model3DCanvasModel.Items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public double GridWidth { get; set; } = 100;

        public double GridHeight { get; set; } = 100;

        //scale on Z Axis
        double explodedScale = 1.0d;
        public double ExplodedScale
        {
            get { return explodedScale; }
            set
            {
                if (explodedScale != value)
                {
                    explodedScale = value;
                    OnPropertyChanged(nameof(ExplodedScale));
                }
            }
        }

        bool showSolderMask = true;
        public bool ShowSolderMask
        {
            get { return showSolderMask; }
            set
            {
                if (showSolderMask != value)
                {
                    showSolderMask = value;
                    OnPropertyChanged(nameof(ShowSolderMask));
                }
            }
        }

        //todo: we need to store this
        XColor solderMaskColor = XColor.FromHexString("#DD008000");
        public XColor SolderMaskColor
        {
            get { return solderMaskColor; }
            set
            {
                if (solderMaskColor != value)
                {
                    solderMaskColor = value;
                    OnPropertyChanged(nameof(SolderMaskColor));
                }
            }
        }

        XColor silkScreenColor = XColors.White;
        public XColor SilkScreenColor
        {
            get { return silkScreenColor; }
            set
            {
                if (silkScreenColor != value)
                {
                    silkScreenColor = value;
                    OnPropertyChanged(nameof(SilkScreenColor));
                }
            }
        }

        bool displayCopperAsLayerColors = false;
        public bool DisplayCopperAsLayerColors
        {
            get { return displayCopperAsLayerColors; }
            set
            {
                if (displayCopperAsLayerColors != value)
                {
                    displayCopperAsLayerColors = value;
                    OnPropertyChanged(nameof(DisplayCopperAsLayerColors));
                }
            }
        }

        //copper color?

        bool showParts = true;
        public bool ShowParts
        {
            get { return showParts; }
            set
            {
                if (showParts != value)
                {
                    showParts = value;
                    OnPropertyChanged(nameof(ShowParts));
                }
            }
        }

        //can hide all layers except parts when set to false
        bool showPCB = true;
        public bool ShowPCB
        {
            get { return showPCB; }
            set
            {
                if (showPCB != value)
                {
                    showPCB = value;
                    OnPropertyChanged(nameof(ShowPCB));
                }
            }
        }

        //true to set dielectric layers visible
        bool showDielectric = true;
        public bool ShowDielectric
        {
            get { return showDielectric; }
            set
            {
                if (showDielectric != value)
                {
                    showDielectric = value;
                    OnPropertyChanged(nameof(ShowDielectric));
                }
            }
        }

        //improvement: show as trully fabricated (with holes in silkscreen, and removed from outside borders)

        public void HandleLayers()
        {
            foreach (var layer in Preview3DLayers)
            {
                HandleLayerIsVisible(layer);
                HandleLayerColor(layer);
                HandleLayerPosition(layer);
            }
        }

        private void HandleLayerColor(Preview3DLayerData layer)
        {
            switch (layer.LayerType)
            {
                case LayerType.SolderMask:
                    layer.DisplayColor = SolderMaskColor;
                    break;

                case LayerType.Signal:
                case LayerType.Plane:
                    layer.DisplayColor = DisplayCopperAsLayerColors ? layer.LayerColor : layer.Color;
                    break;

                case LayerType.SilkScreen:
                    layer.DisplayColor = SilkScreenColor;
                    break;
            }
        }

        private void HandleLayerPosition(Preview3DLayerData layer)
        {
            layer.DisplayPositionZ = layer.PositionZ * ExplodedScale;
        }

        private void HandleLayerIsVisible(Preview3DLayerData layer)
        {
            bool visible = ShowPCB;

            if (ShowPCB)
            {
                switch (layer.LayerType)
                {
                    case LayerType.SolderMask:
                        visible = ShowSolderMask;
                        break;

                    case LayerType.Dielectric:
                        visible = ShowDielectric;
                        break;
                }
            }

            layer.IsVisible = visible;
        }

        protected override void PropertyChangedHandleInternal(object sender, PropertyChangedEventArgs e)
        {
            HandleLayers();
        }
    }

}
