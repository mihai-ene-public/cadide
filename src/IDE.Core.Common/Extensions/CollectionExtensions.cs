using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core
{
    public static class CollectionExtensions
    {
        public static void MoveUp<T>(this IList<T> collection, T item)
        {
            if(collection is ObservableCollection<T> obs)
            {
                obs.MoveUp(item);
            }
        }

        public static void MoveUp<T>(this ObservableCollection<T> collection, T item)
        {
            if (item == null)
                return;
            var oldIndex = collection.IndexOf((T)item);
            var newIndex = oldIndex - 1;
            if (newIndex >= 0)
            {
                collection.Move(oldIndex, newIndex);
            }
        }

        public static void MoveDown<T>(this IList<T> collection, T item)
        {
            if (collection is ObservableCollection<T> obs)
            {
                obs.MoveDown(item);
            }
        }
        public static void MoveDown<T>(this ObservableCollection<T> collection, T item)
        {
            if (item == null)
                return;
            var oldIndex = collection.IndexOf((T)item);
            var newIndex = oldIndex + 1;
            if (newIndex < collection.Count)
            {
                collection.Move(oldIndex, newIndex);
            }
        }
    }
}
