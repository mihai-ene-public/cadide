using IDE.Core.Interfaces;
using IDE.Core.Types.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IDE.Core.Designers
{
    public abstract class SingleLayerBoardCanvasItem : BoardCanvasItemViewModel, ISingleLayerBoardCanvasItem
    {

        ILayerDesignerItem layer;

        [Editor(EditorNames.LayerDesignerItemEditor, EditorNames.LayerDesignerItemEditor)]
        [Display(Order = 1)]
        [MarksDirty]
        public ILayerDesignerItem Layer
        {
            get
            {
                return layer;
            }
            set
            {
                if (layer != value)
                {
                    layer?.Items.Remove(this);
                    layer = value;
                    if (ParentObject == null)
                        layer?.Items.Add(this);

                    if (layer != null)
                    {
                        LayerId = layer.LayerId;
                    }

                    OnPropertyChanged(nameof(Layer));
                }
            }
        }

        public void AssignLayer(ILayerDesignerItem newLayer)
        {
            layer = newLayer;
            if (layer != null)
                LayerId = layer.LayerId;
            OnPropertyChanged(nameof(Layer));
            OnPropertyChanged(nameof(ZIndex));
        }

        public void AssignLayerForced(ILayerDesignerItem newLayer)
        {
            Layer = null;
            Layer = newLayer;
        }

        //intention to hide
        [Browsable(false)]
        public override int ZIndex { get { return GetZIndex(); } }

        protected override int GetZIndex()
        {
            if (layer != null)
                return layer.ZIndex;
            return 0;
        }

        public int GetIdLayer()
        {
            if (layer != null)
                return layer.LayerId;
            return -1;
        }

        public LayerType GetLayerType()
        {
            if (layer != null)
                return layer.LayerType;
            return LayerType.Unknown;
        }


        [Browsable(false)]
        public int LayerId { get; set; }

        public override bool IsMirrored()
        {
            var fp = ParentObject as FootprintBoardCanvasItem;
            if (fp != null)
                return fp.Placement == FootprintPlacement.Bottom;
            else
            {
                if (Layer != null)
                    return Layer.IsBottomLayer;
            }

            return false;
        }

        public ILayerDesignerItem GetLayer(int layerId)
        {
            if (layerId != 0 && LayerDocument != null && LayerDocument.LayerItems != null)
            {
                return LayerDocument.LayerItems.FirstOrDefault(l => l.LayerId == layerId);
            }

            return null;
        }

        public bool ShouldBeOnLayer(ILayerDesignerItem layer)
        {
            if (this is PadThtCanvasItem pad)
            {
                return true;
            }

            return this.Layer == layer;
        }

        public override void LoadLayers()
        {
            Layer = GetLayer(LayerId);
        }

        public override void RemoveFromCanvas()
        {
            Layer = null;
        }
    }
}
