using System.Collections.Generic;

namespace IDE.Core.Spatial2D
{
    public interface ISpatialDatabase<T> : ISpatialIndex<T>
    {
        void Insert(T item);
        void Delete(T item);
        void Clear();

        void BulkLoad(IEnumerable<T> items);
    }
}
