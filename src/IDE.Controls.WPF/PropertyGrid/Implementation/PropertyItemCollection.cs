using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;

namespace IDE.Controls.WPF.PropertyGrid;

public class PropertyItemCollection : ReadOnlyObservableCollection<PropertyItem>
{
    internal static readonly string DisplayNamePropertyName;

    private bool _preventNotification;

    static PropertyItemCollection()
    {
        DisplayNamePropertyName = nameof(PropertyItem.DisplayName);
    }

    public PropertyItemCollection(ObservableCollection<PropertyItem> editableCollection)
      : base(editableCollection)
    {
        EditableCollection = editableCollection;
    }

    public ObservableCollection<PropertyItem> EditableCollection { get; private set; }

    private ICollectionView GetDefaultView()
    {
        return CollectionViewSource.GetDefaultView(this);
    }


    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (_preventNotification)
            return;

        base.OnCollectionChanged(args);
    }

    internal void UpdateItems(IEnumerable<PropertyItem> newItems)
    {
        if (newItems == null)
            throw new ArgumentNullException(nameof(newItems));

        _preventNotification = true;
        var view = GetDefaultView();
        using (view.DeferRefresh())
        {
            view.SortDescriptions.Clear();
            EditableCollection.Clear();
            foreach (var item in newItems)
            {
                EditableCollection.Add(item);
            }

            SortBy(nameof(PropertyItem.PropertyOrder), ListSortDirection.Ascending);
            SortBy(nameof(PropertyItem.DisplayName), ListSortDirection.Ascending);
        }
        _preventNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void SortBy(string name, ListSortDirection sortDirection)
    {
        GetDefaultView().SortDescriptions.Add(new SortDescription(name, sortDirection));
    }

    internal void RefreshView()
    {
        GetDefaultView().Refresh();
    }
}
