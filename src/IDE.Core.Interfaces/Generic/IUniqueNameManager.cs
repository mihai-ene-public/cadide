using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    public interface IUniqueNameManager<T> where T : IUniqueName
    {
        IList<T> Elements { get; }

        T AddNew();
        T Add(T element);
        T Add(string name);
        T Get(string name);

        void Remove(T element);
    }
}