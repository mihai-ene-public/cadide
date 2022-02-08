using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IDE.Core.Interfaces
{
    public enum LineStyle
    {
        Solid,
        Dash,
        Dot,
        DashDot
    }

    public enum LineCap
    {
        Round,
        Flat
    }

    public interface ILineCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double X1 { get; set; }

        double X2 { get; set; }

        double Y1 { get; set; }

        double Y2 { get; set; }

        double Width { get; set; }

    }
}
