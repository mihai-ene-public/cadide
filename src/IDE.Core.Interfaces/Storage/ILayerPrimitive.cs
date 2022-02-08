using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// a primitive that specifies a layer
    /// </summary>
    public interface ILayerPrimitive
    {
        int layerId
        {
            get; set;
        }
    }
}
