namespace IDE.Core.Importers;

public interface IEagleImporter
{
    void Import(string sourceFilePath, string destinationFolder);

    IList<LayerMappingInfo> GetSuggestedLayerMapping(string sourceFilePath);

    void SetSuggestedLayerMapping(IList<LayerMappingInfo> layerMapping);
}
