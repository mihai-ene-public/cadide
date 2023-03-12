using IDE.Core.Storage;

namespace IDE.Core.Importers;

public class LayerMappingInfo
{
    public string SourceLayerId { get; set; }
    public string SourceLayerName { get; set; }

    public int DestLayerId { get; set; }

    public string DestLayerName { get; set; }

    public Layer Layer { get; set; }
}