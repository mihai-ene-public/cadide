using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Interfaces.Geometries
{
    public interface IGeometryOutline
    {
        XTransform Transform { get; set; }
       
        
        List<XPoint> GetOutline();

    }
}
