using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using IDE.Core.Interfaces;

using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces
{
    public interface IPinCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {

        string Number { get; set; }

        bool ShowNumber { get; set; }

        string Name { get; set; }

        bool ShowName { get; set; }

        double X { get; set; }

        double Y { get; set; }

        double Width { get; set; }

        double PinLength { get; set; }

        pinOrientation Orientation { get; set; }

        IPosition PinNamePosition { get; set; }

        IPosition PinNumberPosition { get; set; }

        XColor PinColor { get; set; }

        XColor PinNameColor { get; set; }

        XColor PinNumberColor { get; set; }

    }

    /// <summary>
    /// direction the pin exits. Clockwise
    /// </summary>
    public enum pinOrientation
    {
        /// <summary>
        /// 0 degrees
        /// </summary>
        Left = 0,

        /// <summary>
        /// 90 degrees
        /// </summary>
        Up = 90,

        /// <summary>
        /// 180 degrees
        /// </summary>
        Right = 180,

        /// <summary>
        /// 270 degrees
        /// </summary>
        Down = 270,
    }
}
