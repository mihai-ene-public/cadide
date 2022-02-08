using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using IDE.Documents.Views;

namespace IDE.Controls
{
    /// <summary>
    ///     Represents a model that can be used to present a collection of items. supports generating child items by a
    ///     <see cref="DataTemplate" />.
    /// </summary>
    /// <remarks>
    ///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsControl. You can set the ItemsSource
    ///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
    ///     ItemsControl to a collection object.
    /// </remarks>
    public class ItemsModel3D : CompositeModel3D
    {
        /// <summary>
        ///     The item template property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate), typeof(ItemsModel3D), new PropertyMetadata(null));

        /// <summary>
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsModel3D),
            new PropertyMetadata(null, (s, e) => ((ItemsModel3D)s).ItemsSourceChanged(e)));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManagerWrapper),
            typeof(ItemsModel3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as ItemsModel3D;
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
                (d.SceneNode as GroupNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate" /> used to display each item.
        /// </summary>
        /// <value>
        ///     The item template.
        /// </value>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the <see cref="ItemsModel3D" />.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public IOctreeManagerWrapper OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManagerWrapper)GetValue(OctreeManagerProperty);
            }
        }

        private IOctreeBasic Octree
        {
            get { return (SceneNode as GroupNode)?.OctreeManager?.Octree; }
        }

        private readonly Dictionary<object, Element3D> elementDict = new Dictionary<object, Element3D>();

        /// <summary>
        /// Handles changes in the ItemsSource property.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot create a Model3D from ItemTemplate.
        /// </exception>
        private void ItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemsModel3D_CollectionChanged;
            }
            if (e.OldValue == null && e.NewValue != null && Children.Count > 0)
            {
                throw new InvalidOperationException("Children must be empty before using ItemsSource");
            }

            elementDict.Clear();
            Children.Clear();

            if (e.NewValue is INotifyCollectionChanged n)
            {
                n.CollectionChanged -= ItemsModel3D_CollectionChanged;
                n.CollectionChanged += ItemsModel3D_CollectionChanged;
            }

            if (ItemsSource == null)
            {
                return;
            }

            AddItemsFromSource(ItemsSource);
           
            if (Children.Count > 0)
            {
                (SceneNode as GroupNode).OctreeManager?.RequestRebuild();
            }
        }
        private void AddItemsFromSource(IEnumerable source)
        {
            var itemTemplate = ItemTemplate;
            foreach (var item in source)
            {
                if (item is Element3D model)
                {
                    elementDict.Add(item, model);
                    Children.Add(model);
                }
                else
                {
                    var template = itemTemplate ?? GetDataTemplate(item);
                    AddItemFromDataTemplate(item, template);
                }
            }
        }

        private DataTemplate GetDataTemplate(object item)
        {
            // go through all types and base types to find a matching DataTemplate
            // this mirrors the behavior of DataTemplate
            foreach (var type in item.GetType().GetTypeAndBaseTypes())
            {
                var key = type;
                var template = TryFindResource(key) as DataTemplate;
                if (template != null)
                    return template;
            }

            return null;
        }

        private void AddItemFromDataTemplate(object item, DataTemplate template)
        {
            if (template != null)
            {
                var templateContent = template.LoadContent() as Element3D;
                templateContent.DataContext = item;
                elementDict.Add(item, templateContent);
                Children.Add(templateContent);
            }
            else
            {
                Debug.WriteLine("Cannot create a Model3D from ItemTemplate.");
                //throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
            }
        }

        protected void ItemsModel3D_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (elementDict.TryGetValue(item, out var model))
                            {
                                elementDict.Remove(item);
                                Children.Remove(model);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    elementDict.Clear();
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (this.ItemsSource != null)
                    {
                        if (this.ItemTemplate == null)
                        {
                            foreach (var item in this.ItemsSource)
                            {
                                if (item is Element3D model)
                                {
                                    elementDict.Add(item, model);
                                    Children.Add(model);
                                }
                                else
                                {
                                    Debug.WriteLine("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in this.ItemsSource)
                            {
                                if (this.ItemTemplate.LoadContent() is Element3D model)
                                {
                                    model.DataContext = item;
                                    elementDict.Add(item, model);
                                    Children.Add(model);
                                }
                                else
                                {
                                    Debug.WriteLine("Cannot create a Model3D from ItemTemplate.");
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        AddItemsFromSource(e.NewItems);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    Children.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }


        protected override void Dispose(bool disposing)
        {
            elementDict.Clear();
            base.Dispose(disposing);
        }
    }

}
