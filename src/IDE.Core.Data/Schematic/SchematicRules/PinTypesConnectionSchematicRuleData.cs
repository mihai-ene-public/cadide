using System.Collections.Generic;


namespace IDE.Core.Storage
{
    public class PinTypesConnectionSchematicRuleData : SchematicRuleData
    {
        public List<PinTypesConnectionResponseSpec> PinTypesConnections { get; set; } = new List<PinTypesConnectionResponseSpec>();
    }
}
