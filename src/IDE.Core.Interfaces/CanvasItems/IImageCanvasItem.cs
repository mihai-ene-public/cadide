using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface IImageCanvasItem : INotifyPropertyChanged, ISelectableItem
    {
        double X { get; set; }
        double Y { get; set; }

        double Rot { get; set; }

        double Width { get; set; }
        double Height { get; set; }

        double BorderWidth { get; set; }

        XColor BorderColor { get; set; }

        XColor FillColor { get; set; }

        XStretch Stretch { get; set; }

        byte[] ImageBytes { get; set; }
    }
}
