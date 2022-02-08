namespace IDE.Core.Importers
{
    public class DxfLayerMappingItem
    {
        public string SourceLayerName { get; set; }

        public int DestLayerId { get; set; }

        public string DestLayerName { get; set; }
    }
}
