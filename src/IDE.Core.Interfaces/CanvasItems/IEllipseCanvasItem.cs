using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IEllipseCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        double BorderWidth { get; set; }
    }
}
