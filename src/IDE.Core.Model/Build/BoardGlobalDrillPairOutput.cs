using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using System.Collections.Generic;

namespace IDE.Core.Build
{
    public class BoardGlobalDrillPairOutput
    {
        public ILayerPairModel DrillPair { get; set; }

        public IList<GlobalPrimitive> MillingPrimitives { get; set; }
        public IList<GlobalPrimitive> DrillPrimitives { get; set; }
    }
}
