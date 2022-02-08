using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IDE.Core.Interfaces
{
    public interface IRegionCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        XPoint StartPoint { get; }

        double Width { get; }

        ObservableCollection<IRegionItem> Items { get; set; }
    }

    public interface IRegionItem
    {
        double EndPointX { get; set; }
        double EndPointY { get; set; }

        XPoint EndPoint { get; }

        IRegionPrimitive ToPrimitive();
    }

    public interface ILineRegionItem : IRegionItem
    {

    }
    public interface IArcRegionItem : IRegionItem
    {
        bool IsLargeArc { get; set; }

        XSweepDirection SweepDirection { get; set; }

        double SizeDiameter { get; set; }
    }


}
