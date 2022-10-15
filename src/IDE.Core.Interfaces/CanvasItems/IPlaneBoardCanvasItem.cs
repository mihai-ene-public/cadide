using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces.Geometries;

namespace IDE.Core.Interfaces;

public interface IPlaneBoardCanvasItem : ISignalPrimitiveCanvasItem, IGenerateThermals, IExcludeItems, IRepourGeometry
{
    IGeometryOutline RegionGeometry { get; }
}

public interface IRepourGeometry
{
    Task RepourPolygonAsync();
}
