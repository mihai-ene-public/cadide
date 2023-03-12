namespace IDE.Core.Importers;

/// <summary>
/// Information for a layer coming from a source document to import (e.g. footprint or board in Eagle)
/// </summary>
public class SourceLayer
{
    public string LayerId { get; set; }
    public string LayerName { get; set; }
}
