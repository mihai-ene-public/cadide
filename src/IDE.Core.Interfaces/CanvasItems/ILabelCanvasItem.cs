using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface ILabelCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        //double Left { get; set; }//?
        //double Top { get; set; }

        double X { get; set; }

        double Y { get; set; }

        double Rot { get; set; }

        string Text { get; }

        XColor TextColor { get; set; }

        string FontFamily { get; set; }//will be converted to string

        double FontSize { get; set; }

        bool Bold { get; set; }

        bool Italic { get; set; }
    }
}
