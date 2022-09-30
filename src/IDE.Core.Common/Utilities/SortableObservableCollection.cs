using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using IDE.Core.Types.Input;

namespace IDE.Core
{

    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public SortableObservableCollection()
        {
        }

        public SortableObservableCollection(IComparer<T> comparer, string itemPropertyName = "Name")
        {
            _comparer = comparer;
            _itemPropertyName = itemPropertyName;
            _autosort = true;
        }

        private readonly IComparer<T> _comparer;
        private readonly string _itemPropertyName;
        private bool _autosort;

        public void SortAscending<TKey>(Func<T, TKey> keySelector)
        {
            Sort(keySelector, ListSortDirection.Ascending);
        }

        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            Sort(keySelector, ListSortDirection.Descending);
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        public void PauseAutosort()
        {
            _autosort = false;
        }
        public void ResumeAutosort()
        {
            _autosort = true;
        }

        private void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged observable)
                    {
                        observable.PropertyChanged += Observable_PropertyChanged;
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged observable)
                    {
                        observable.PropertyChanged -= Observable_PropertyChanged;
                    }
                }
            }

            if (_autosort && e.Action == NotifyCollectionChangedAction.Add)
            {
                if (_comparer != null)
                {
                    ApplySort(Items.OrderBy(item => item, _comparer));
                }
            }

        }

        private void Observable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_itemPropertyName == e.PropertyName)
            {
                ExecuteSort();
            }
        }

        private void ExecuteSort()
        {
            if (_autosort)
            {
                if (_comparer != null)
                {
                    ApplySort(Items.OrderBy(item => item, _comparer));
                }
            }
        }

    }
}
