using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface IRectangleCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }

        double Rot { get; set; }

        double CornerRadius { get; set; }

        double BorderWidth { get; set; }

        bool IsFilled { get; set; }


    }

    public interface IRectangleSchematicCanvasItem:IRectangleCanvasItem
    {
        
            XColor BorderColor { get; set; }

            XColor FillColor { get; set; }
    }
}
