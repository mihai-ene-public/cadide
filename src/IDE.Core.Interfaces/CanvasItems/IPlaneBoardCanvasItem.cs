using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface IPlaneBoardCanvasItem : ISignalPrimitiveCanvasItem, IGenerateThermals, IExcludeItems, IRepourGeometry
    {
        IGeometry RegionGeometry { get; }
    }

    public interface IRepourGeometry
    {
        Task RepourPolygonAsync();
    }
}
