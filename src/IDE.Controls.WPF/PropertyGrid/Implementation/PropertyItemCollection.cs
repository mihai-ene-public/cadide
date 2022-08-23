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
        PropertyItem p = null;
        DisplayNamePropertyName = nameof(p.DisplayName);
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
        using (GetDefaultView().DeferRefresh())
        {
            EditableCollection.Clear();
            foreach (var item in newItems)
            {
                EditableCollection.Add(item);
            }
        }
        _preventNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    internal void RefreshView()
    {
        GetDefaultView().Refresh();
    }
}
