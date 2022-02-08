namespace IDE.Core
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Helper class to find a child item of a given item in the Visual Tree of WPF 
    /// </summary>
    public static class VisualTreeHelperExtension
    {
        /// <summary>
        /// Looks for a child control within a parent by name
        /// </summary>
        public static DependencyObject FindChild(this DependencyObject parent, string name)
        {
            // confirm parent and name are valid.
            if (parent == null || string.IsNullOrEmpty(name))
                return null;

            var frameworkElement = parent as FrameworkElement;
            if (frameworkElement != null && frameworkElement.Name == name)
                return parent;

            DependencyObject result = null;

            if (frameworkElement != null)
                frameworkElement.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                result = FindChild(child, name);
                if (result != null)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        public static T FindChild<T>(this DependencyObject parent)
                where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null)
                return null;
            if (parent is T)
                return parent as T;

            DependencyObject foundChild = null;

            var frameworkElement = parent as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null)
                    break;
            }

            return foundChild as T;
        }

        public static T FindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            if (child == null) return null;

            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            if (parent is T)
                return parent as T;

            return FindParent<T>(parent);
        }


        public static T FindParentDataContext<T>(this DependencyObject child)
            where T : class
        {
            if (child == null) return null;

            var parent = VisualTreeHelper.GetParent(child) as FrameworkElement;
            if (parent == null) return null;
            if (parent.DataContext is T)
                return parent.DataContext as T;

            return FindParentDataContext<T>(parent);
        }

        public static T FindDataContext<T>(this FrameworkElement element)
            where T : class
        {
            if (element == null) return null;

            if(element.DataContext is T)
                return element.DataContext as T;

            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
           
            return FindDataContext<T>(parent);
        }
    }
}
