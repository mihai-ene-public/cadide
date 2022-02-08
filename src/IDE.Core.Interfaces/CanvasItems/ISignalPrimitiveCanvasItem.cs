using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISignalPrimitiveCanvasItem : ISelectableItem
    {
        IBoardNetDesignerItem Signal { get; set; }

        void AssignSignal(IBoardNetDesignerItem newSignal);
    }
}
