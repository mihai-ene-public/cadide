using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using System;
using System.Linq;
using System.Windows;


namespace IDE.Core.Resources;

public class ResourceLocator : IResourceLocator
{
    public static T GetResource<T>(string assemblyName, string resourceFilename) where T : class
    {
        return GetResource<T>(assemblyName, resourceFilename, string.Empty);
    }

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
                catch
                {
                    throw;
                }
            }
        }
    }
}
