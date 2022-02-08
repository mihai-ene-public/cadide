using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IHoleCanvasItem : ISelectableItem
    {
        double X { get; set; }

        double Y { get; set; }

        double Drill { get; set; }

        double Height { get; set; }

        double Rot { get; set; }

        DrillType DrillType { get; set; }
    }

}
