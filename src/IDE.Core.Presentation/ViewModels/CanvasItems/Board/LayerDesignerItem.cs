using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers
{

    /// <summary>
    /// this is a layer shown in Layers tool window
    /// </summary>
    public class LayerDesignerItem : BaseCanvasItem, ILayerDesignerItem
    {
        //layerId
        //?isVisible
        //layerType: Signal, Plane, Mechanical, Mask(Paste, Solder), Silkscreen (overlay), Other (Drill, Keepout, Multi-Layer)
        //          Multi Layer, Paste Mask, Silk Screen, Solder Mask, Signal, Drill Guide, Keep-out, Mechanical, Drill Drawing

        public const int SelectedLayerZIndex = 4000;

        public LayerDesignerItem(ILayeredViewModel layeredModel)
        {
            layeredFileModel = layeredModel;
            CanEdit = false;
            IsVisible = true;
            IsMasked = false;

        }

        ILayeredViewModel layeredFileModel;

        int layerId;
        public int LayerId
        {
            get { return layerId; }

            set
            {
                layerId = value;
                OnPropertyChanged(nameof(LayerId));
            }
        }

        int stackOrder;
        public int StackOrder
        {
            get { return stackOrder; }

            set
            {
                stackOrder = value;
                OnPropertyChanged(nameof(StackOrder));
            }
        }

        XColor layerColor;
        public XColor LayerColor
        {
            get { return layerColor; }

            set
            {
                layerColor = value;
                OnPropertyChanged(nameof(LayerColor));
            }
        }

        float dimmColorAmount = 0.25f;
        public XColor DimmedColor
        {
            get
            {
                var c = layerColor * dimmColorAmount;
                c.A = 255;
                return c;//.ToColor();
            }
        }

        string layerName;
        public string LayerName
        {
            get { return layerName; }
            set
            {
                layerName = value;
                OnPropertyChanged(nameof(LayerName));
            }
        }

        LayerType layerType;
        public LayerType LayerType
        {
            get { return layerType; }
            set
            {
                layerType = value;
                OnPropertyChanged(nameof(LayerType));
            }
        }

        #region Gerber Output

        bool plot = true;
        public bool Plot
        {
            get { return plot; }
            set
            {
                plot = value;
                OnPropertyChanged(nameof(Plot));
            }
        }

        bool mirrorPlot;
        public bool MirrorPlot
        {
            get { return mirrorPlot; }
            set
            {
                mirrorPlot = value;
                OnPropertyChanged(nameof(MirrorPlot));
            }
        }

        string gerberFileName;
        public string GerberFileName
        {
            get { return gerberFileName; }
            set
            {
                gerberFileName = value;
                OnPropertyChanged(nameof(GerberFileName));
            }
        }

        string gerberExtension = "GBR";
        //".GBR"
        public string GerberExtension
        {
            get { return gerberExtension; }
            set
            {
                gerberExtension = value;
                OnPropertyChanged(nameof(GerberExtension));
            }
        }

        #endregion

        public string Material { get; set; }

        double thickness = 0.01;
        public double Thickness
        {
            get { return thickness; }
            set
            {
                thickness = value;
                OnPropertyChanged(nameof(Thickness));
            }
        }

        public double DielectricConstant { get; set; }

        bool isVisible;
        [Browsable(false)]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible == value) return;
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        bool isSetVisible = true;
        public bool IsSetVisible
        {
            get { return isSetVisible; }
            set
            {
                if (isSetVisible == value) return;
                isSetVisible = value;
                OnPropertyChanged(nameof(IsSetVisible));
                HandleLayer();
            }
        }

        protected override int GetZIndex()
        {
            if (IsCurrentLayer)
                return SelectedLayerZIndex;
            return LayerOrder;
        }

        bool isCurrentLayer;
        public bool IsCurrentLayer
        {
            get { return isCurrentLayer; }
            set
            {
                isCurrentLayer = value;
                OnPropertyChanged(nameof(IsCurrentLayer));
                OnPropertyChanged(nameof(ZIndex));
            }
        }

        IList<ISelectableItem> items = new SpatialItemsSource();
        public IList<ISelectableItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }


        /// <summary>
        /// order from bottom to top
        /// 0 - on bottom; 1 - higher order than bottom
        /// </summary>
        [Browsable(false)]
        public int LayerOrder { get; set; } = 0;

        bool isDimmed;
        [Browsable(false)]
        public bool IsDimmed
        {
            get { return isDimmed; }
            set
            {
                if (isDimmed == value) return;
                isDimmed = value;
                OnPropertyChanged(nameof(IsDimmed));
            }
        }

        bool isMasked;
        [Browsable(false)]
        public bool IsMasked
        {
            get { return isMasked; }
            set
            {
                if (isMasked == value) return;
                isMasked = value;
                OnPropertyChanged(nameof(IsMasked));
            }
        }

        [Browsable(false)]
        public bool IsBottomLayer
        {
            get
            {
                var bottomLayers = new[]
                {
                    LayerConstants.SignalBottomLayerId,
                    LayerConstants.SilkscreenBottomLayerId,
                    LayerConstants.SolderBottomLayerId,
                    LayerConstants.PasteBottomLayerId
                };

                return bottomLayers.Contains(layerId);
            }
        }

        [Browsable(false)]
        public bool IsTopLayer
        {
            get
            {
                var topLayers = new[]
                {
                    LayerConstants.SignalTopLayerId,
                    LayerConstants.SilkscreenTopLayerId,
                    LayerConstants.SolderTopLayerId,
                    LayerConstants.PasteTopLayerId
                };

                return topLayers.Contains(layerId);
            }
        }

        public bool HideLayerWhenDimmed { get; set; }

        //LayersToolWindowViewModel layersWindow;
        //protected LayersToolWindowViewModel LayersWindow
        //{
        //    get
        //    {
        //        if (layersWindow == null)
        //        {
        //            layersWindow = ServiceProvider.GetService<LayersToolWindowViewModel>();
        //            //if (layersWindow != null)
        //            //    layersWindow.PropertyChanged += LayersWindow_PropertyChanged;
        //        }
        //        return layersWindow;
        //    }
        //}

        //void LayersWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //   // HandleLayer();
        //}

        public void HandleLayer()
        {
            if (layeredFileModel != null && layeredFileModel.SelectedLayerGroup != null)
                HandleLayer(layeredFileModel.SelectedLayerGroup.Layers,
                            layeredFileModel.SelectedLayer,
                            layeredFileModel.HideUnselectedLayer,
                            layeredFileModel.MaskUnselectedLayer);
        }

        void HandleLayer(IList<ILayerDesignerItem> visibleLayers, ILayerDesignerItem selectedLayer, bool hideLayerNotSelected = false, bool dimmLayerNotSelected = false)
        {
            try
            {
                if (visibleLayers == null || selectedLayer == null)
                    return;

                //turn IsVis=false if this layer does not belong to current layer group
                var isLayerOnLayerGroup = visibleLayers.Any(l => l.LayerId == LayerId);
                if (!isLayerOnLayerGroup)
                {
                    IsVisible = false;
                    IsCurrentLayer = false;
                    return;
                }

                var isItemOnSelectedLayer = selectedLayer.LayerId == LayerId;
                IsCurrentLayer = isItemOnSelectedLayer;
                //hide item if we are not on selected layer and the option to hide is enabled
                if (!isItemOnSelectedLayer)
                {
                    if (hideLayerNotSelected)
                    {
                        IsVisible = false;
                        return;
                    }
                    if (dimmLayerNotSelected)
                    {
                        if (HideLayerWhenDimmed)
                        {
                            IsVisible = false;
                        }
                        else
                        {
                            IsMasked = true;
                            IsVisible = isSetVisible;
                        }
                        return;
                    }
                }

                IsVisible = isSetVisible;//true
                IsMasked = false;
            }
            finally
            {
                OnPropertyChanged(nameof(ZIndex));
            }


        }


        public override string ToString()
        {
            return LayerName;
        }

        public void CreateLayerId(IList<ILayerDesignerItem> existingLayers)
        {
            // we could have some posible bugs here

            if (existingLayers == null)
                return;

            var layers = existingLayers.Where(l => l.LayerType == layerType).Select(l => l.LayerId).ToList();
            var foundId = false;
            var currentId = (int)LayerType + 1;
            while (!foundId)
            {
                if (layers.Contains(currentId))
                {
                    currentId++;
                }
                else
                {
                    foundId = true;
                }
            }

            //we don't allow to exceed to the next layer typ
            if (currentId / (((int)LayerType) + 100) >= 1)
                throw new Exception($"Available number of layers was exceeded for layer type: {LayerType}");

            //we don't allow to override the bottom
            if (layerType == LayerType.Signal && currentId == LayerConstants.SignalBottomLayerId)
                throw new Exception($"Available number of layers was exceeded for layer type: {LayerType}");

            LayerId = currentId;
        }

        public override XRect GetBoundingRectangle()
        {
            return XRect.Empty;
        }

        public override void Translate(double dx, double dy)
        {
        }

        //public override Adorner CreateAdorner(UIElement element)
        //{
        //    return null;
        //}

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void Rotate()
        {
        }
    }
}
