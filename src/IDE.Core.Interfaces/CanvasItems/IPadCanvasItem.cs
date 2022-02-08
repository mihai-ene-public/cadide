using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IPadCanvasItem : ISignalPrimitiveCanvasItem//ICanvasItem, ISelectableItem
    {
        string Number { get; set; }

        long FootprintInstanceId { get; }

        double Width { get; set; }

        double Height { get; set; }
        double CornerRadius { get; }// set; }

        double X { get; set; }

        double Y { get; set; }

        double Rot { get; set; }

        bool AutoGenerateSolderMask { get; set; }

        bool AdjustSolderMaskInRules { get; set; }

        bool AutoGeneratePasteMask { get; set; }

        bool AdjustPasteMaskInRules { get; set; }
    }

    //public interface IPadWithDrillCanvasItem : IPadCanvasItem
    //{
    //    double Drill { get; set; }
    //}
}
