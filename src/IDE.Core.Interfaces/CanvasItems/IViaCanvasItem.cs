using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IViaCanvasItem : ISignalPrimitiveCanvasItem
    {
        double X { get; }

        double Y { get; }

        double Drill { get; }

        double Diameter { get; }

        ILayerPairModel DrillPair { get; }
    }
}
