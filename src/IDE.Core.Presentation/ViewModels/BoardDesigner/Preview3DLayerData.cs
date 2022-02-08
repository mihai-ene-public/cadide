using System.Collections.Generic;
using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views
{
    public class Preview3DLayerData : BaseViewModel
    {
        public byte Alpha { get { return DisplayColor.A; } }

        public bool IsTransparent => Alpha < 255;

        public IMeshModel Mesh { get; set; }

        //color that should be displayed
        XColor displayColor;
        public XColor DisplayColor
        {
            get { return displayColor; }
            set
            {
                if(displayColor!=value)
                {
                    displayColor = value;
                    OnPropertyChanged(nameof(DisplayColor));
                    OnPropertyChanged(nameof(Alpha));
                }
            }
        }
        public XColor Color { get; set; }//initial color
        public XColor LayerColor { get; set; }//color from the board layers

        public LayerType LayerType { get; set; }

        public double PositionZ { get; set; }

        double displayPositionZ;
        public double DisplayPositionZ
        {
            get { return displayPositionZ; }
            set
            {
                if (displayPositionZ != value)
                {
                    displayPositionZ = value;
                    OnPropertyChanged(nameof(DisplayPositionZ));
                }
            }
        }

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }
    }

}
