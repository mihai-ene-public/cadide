using System.Collections.Generic;
using IDE.Core.Types.Media;

namespace IDE.Core.Build;

public class BuildGlobalResult
{
    public IList<BoardGlobalLayerOutput> Layers { get; set; }

    public IList<BoardGlobalDrillPairOutput> DrillLayers { get; set; }

    public XRect BodyRectangle { get; set; }
}
