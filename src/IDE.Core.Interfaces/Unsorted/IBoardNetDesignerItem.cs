using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IBoardNetDesignerItem : INotifyPropertyChanged
    {
        long Id { get; set; }

        string Name { get; set; }
        long ClassId { get; set; }

        bool IsHighlighted { get; set; }

        bool IsNamed();

        void HighlightNet(bool newHighlight);

        IList<ISelectableItem> Items { get; set; }

        IList<IPadCanvasItem> Pads { get; set; }
    }
}
