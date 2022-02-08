using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class BoardLayerOutput
    {
        public ILayerDesignerItem Layer { get; set; }

        public IEnumerable<ICanvasItem> AddItems { get; set; }
        public IEnumerable<ICanvasItem> ExtractItems { get; set; }
    }
}
