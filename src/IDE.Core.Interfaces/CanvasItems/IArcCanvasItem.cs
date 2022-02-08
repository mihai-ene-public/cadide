using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IDE.Core.Interfaces
{
    public interface IArcCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double StartPointX { get; set; }
        double StartPointY { get; set; }

        double EndPointX { get; set; }
        double EndPointY { get; set; }

        double BorderWidth { get; set; }

        bool IsFilled { get; }

        XSweepDirection SweepDirection { get; set; }

        bool IsLargeArc { get; }

        double RotationAngle { get; }

        XSize Size { get; }

        double Radius { get; set; }

        XPoint GetCenter();

        bool IsMirrored();
    }
}
