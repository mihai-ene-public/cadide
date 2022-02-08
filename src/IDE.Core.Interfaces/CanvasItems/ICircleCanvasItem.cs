using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ICircleCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Diameter { get; set; }

        bool IsFilled { get; set; }

        double BorderWidth { get; set; }
    }
}
