using System.Collections.Generic;

namespace IDE.Core.Spatial2D
{
    public interface ISpatialIndex<T>
    {
        IList<T> Search();
        IList<T> Search(Envelope boundingBox);
    }
}
