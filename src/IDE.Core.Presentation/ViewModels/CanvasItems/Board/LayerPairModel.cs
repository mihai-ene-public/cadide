using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class LayerPairModel : BaseViewModel, ILayerPairModel
    {
        ILayerDesignerItem layerStart;
        public ILayerDesignerItem LayerStart
        {
            get { return layerStart; }
            set
            {
                layerStart = value;
                OnPropertyChanged(nameof(LayerStart));
            }
        }

        ILayerDesignerItem layerEnd;
        public ILayerDesignerItem LayerEnd
        {
            get { return layerEnd; }
            set
            {
                layerEnd = value;
                OnPropertyChanged(nameof(LayerEnd));
            }
        }

        public override string ToString()
        {
            if (layerStart != null && layerEnd != null)
                return $"{layerStart.LayerName} - {layerEnd.LayerName}";

            return base.ToString();
        }
    }
}
