using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using System.Collections.Generic;

namespace IDE.Core.Build
{
    public class BoardGlobalLayerOutput
    {
        public ILayerDesignerItem Layer { get; set; }

        public GlobalPrimitive BoardOutline { get; set; }

        public IList<GlobalPrimitive> AddItems { get; set; }
        public IList<GlobalPrimitive> ExtractItems { get; set; }
    }
}
