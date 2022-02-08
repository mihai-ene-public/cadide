using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces
{
    public interface IJunctionCanvasItem : INotifyPropertyChanged, ISelectableItem, ICanvasItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Diameter { get; set; }

        XColor Color { get; set; }
    }

    

   

    
}
