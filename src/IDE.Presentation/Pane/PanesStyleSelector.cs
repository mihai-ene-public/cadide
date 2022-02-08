namespace IDE.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Select a tool window style for an instance of its view.
    /// </summary>
    public class PanesStyleSelector : StyleSelector
    {
        private Dictionary<Type, Style> styleDirectory = null;

        public PanesStyleSelector()
        {
        }

        public override Style SelectStyle(object item,
                                          DependencyObject container)
        {
            if (styleDirectory == null)
                return null;

            if (item == null)
                return null;

            Style o;
            var t = item.GetType();
            styleDirectory.TryGetValue(t, out o);

            if (o != null)
                return o;

            // Get next base of the current type in inheritance tree
            var t1 = item.GetType().BaseType;

            // Traverse backwards in the inheritance chain to find a mapping there
            while (t1 != t && t != null)
            {
                t = t1;
                styleDirectory.TryGetValue(t, out o);

                if (o != null)
                    return o;

                t1 = t1.BaseType;
            }

            return base.SelectStyle(item, container);
        }

        public void RegisterStyle(Type typeOfViewmodel, Style styleOfView)
        {
            if (styleDirectory == null)
                styleDirectory = new Dictionary<Type, Style>();

            styleDirectory[typeOfViewmodel] = styleOfView;
        }
    }
}
