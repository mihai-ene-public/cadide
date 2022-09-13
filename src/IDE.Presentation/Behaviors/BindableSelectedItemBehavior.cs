using IDE.Core.Utilities;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IDE.Core.Behaviors
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItem Property

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
                                    DependencyProperty.Register("SelectedItem",
                                                                typeof(object),
                                                                typeof(BindableSelectedItemBehavior),
                                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tree = (sender as BindableSelectedItemBehavior).AssociatedObject;
            if (tree == null)
                return;

            if (e.NewValue is TreeViewItem)
            {
                var item = e.NewValue as TreeViewItem;
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
            //else if (e.NewValue is TreeNodeBaseItem)
            //{
            //    var item = e.NewValue as TreeNodeBaseItem;
            //    item.IsSelected = true;
            //}
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dpSource = e.OriginalSource as FrameworkElement;

            //unselect an item if we click on empty space
            if (e.ChangedButton == MouseButton.Left && AssociatedObject.SelectedItem != null)
            {
                var hovered = AssociatedObject.GetDataFromTreeControl<BaseViewModel>(dpSource);
                if (hovered == null)
                {
                    ClearTreeViewItemsControlSelection(AssociatedObject.Items, AssociatedObject.ItemContainerGenerator);
                }
            }

        }

        void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if (ic != null && icg != null)
                for (int i = 0; i < ic.Count; i++)
                {
                    var tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (tvi != null)
                    {
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        tvi.IsSelected = false;
                    }
                }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
                AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }
    }
}
