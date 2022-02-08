using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISymbolCanvasItem: ISelectableItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Rot { get; set; }

        string PartName { get; set; }

        string SymbolName { get; }
        bool ShowName { get; set; }

        IPosition SymbolNamePosition { get; set; }

        IPosition CommentPosition { get; set; }

        string Comment { get; set; }

        bool ShowComment { get; set; }

        IList<ISelectableItem> Items { get; }
    }
}
