namespace IDE.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Data template selector that can be used to select a <seealso cref="DataTemplate"/>
    /// for view instance based on a viewmodel (class) type.
    /// </summary>
    public class PanesTemplateSelector : DataTemplateSelector
    {
        #region fields
        private Dictionary<Type, DataTemplate> templateDirectory = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public PanesTemplateSelector()
        {
        }
        #endregion constructor

        #region methods
        /// <summary>
        /// Returns a System.Windows.DataTemplate based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a System.Windows.DataTemplate or null. The default value is null.</returns>
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (templateDirectory == null)
                return null;

            if (item == null)
                return null;

            DataTemplate o;
            templateDirectory.TryGetValue(item.GetType(), out o);

            if (o != null)
                return o;

            return base.SelectTemplate(item, container);
        }

        /// <summary>
        /// Register a (viewmodel) class type with a <seealso cref="DataTemplate"/> for a view.
        /// </summary>
        /// <param name="typeOfViewmodel"></param>
        /// <param name="view"></param>
        public void RegisterDataTemplate(Type typeOfViewmodel, DataTemplate view)
        {
            if (templateDirectory == null)
                templateDirectory = new Dictionary<Type, DataTemplate>();

            if (view != null)
                templateDirectory[typeOfViewmodel] = view;
               // templateDirectory.Add(typeOfViewmodel, view);
        }
        #endregion methods
    }
}
