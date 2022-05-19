using System.Collections.Generic;

namespace IDE.Core.Build;

public class BuildGlobalResult
{
    public IList<BoardGlobalLayerOutput> Layers { get; set; }

    public IList<BoardDrillPairOutput> DrillLayers { get; set; }
}
