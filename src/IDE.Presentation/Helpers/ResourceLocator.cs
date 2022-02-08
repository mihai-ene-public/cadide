namespace IDE.Core.Resources
{
    using IDE.Core.Interfaces;
    using System;
    using System.Linq;
    using System.Windows;
    using Utilities;

    /// <summary>
    /// Locate resources ín any assembly and return their reference.
    /// This class can, for example, be used to load a DataTemplate instance from an XAML reference.
    /// That is, the XAML is referenced as URI string (and the XAML itself can live in an extra assembly).
    /// The returned instance can be consumed in a 'code behind' context.
    /// </summary>
    public class ResourceLocator : IResourceLocator
    {
       // private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets the first matching resource of the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <returns></returns>
        public static T GetResource<T>(string assemblyName, string resourceFilename) where T : class
        {
            return GetResource<T>(assemblyName, resourceFilename, string.Empty);
        }

        /// <summary>
        /// Gets the resource by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetResource<T>(string assemblyName, string resourceFilename, string name) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(resourceFilename))
                    return default(T);

                string uriPath = string.Format("/{0};component/{1}", assemblyName, resourceFilename);
                var uri = new Uri(uriPath, UriKind.Relative);
                var resource = Application.LoadComponent(uri) as ResourceDictionary;

                if (resource == null)
                    return default(T);

                if (!string.IsNullOrEmpty(name))
                {
                    if (resource.Contains(name))
                        return resource[name] as T;

                    return default(T);
                }

                return resource.Values.OfType<T>().FirstOrDefault();
            }
            catch (Exception exp)
            {
               // logger.Error(string.Format("Error Loading resource '{0}': {1}", "Exception:", exp.Message, exp));

                MessageDialog.Show(exp.Message, "Error loading internal resource.", XMessageBoxButton.OK);
            }

            return default(T);
        }

        public T FindResource<T>(string assemblyName, string resourceFilename, string name) where T : class
        {
            return GetResource<T>(assemblyName, resourceFilename, name);
        }

        public void SwitchToSelectedTheme()
        {
            var themesManager = ServiceProvider.Resolve<IThemesManager>();

            var theme = themesManager.SelectedTheme;

            if (theme != null)
            {
                Application.Current.Resources.MergedDictionaries.Clear();

                foreach (var item in theme.Resources)
                {
                    try
                    {
                        var Res = new Uri(item, UriKind.Relative);

                        var dictionary = Application.LoadComponent(Res) as ResourceDictionary;

                        if (dictionary != null)
                            Application.Current.Resources.MergedDictionaries.Add(dictionary);
                    }
                    catch// (Exception ex)
                    {
                        throw;
                        //MessageDialog.Show(ex.Message);
                    }
                }
            }
        }
    }
}
