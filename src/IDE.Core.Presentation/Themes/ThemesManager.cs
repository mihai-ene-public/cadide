using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IDE.Core.Themes;

/// <summary>
/// This class manages a list of WPF themes (Aero, Metro etc) which
/// can be combined with TextEditorThemes (True Blue, Deep Black).
/// 
/// The class implements a service that can be accessed via Instance property.
/// The exposed methodes and properties can be used to display a list available
/// themes, determine the currently selected theme, and set the currently selected
/// theme.
/// </summary>
public class ThemesManager : IThemesManager, IParentSelectedTheme
{
    const string MetroDarkThemeName = "Metro Dark";

    static readonly string[] MetroDarkResources =
{
  "/IDE.Presentation;component/_Themes/MetroDark/Theme.xaml",
};

    public const string DefaultThemeName = MetroDarkThemeName;

    private SortedDictionary<string, ThemeBase> textEditorThemes = null;
    private IList<ITheme> listOfAllThemes = null;
   

    public ThemesManager()
    {
    }

    #region properties

    private string selectedThemeName = DefaultThemeName;

    public string SelectedThemeName
    {
        get
        {
            return selectedThemeName;
        }
    }

    public ITheme SelectedTheme
    {
        get
        {
            if (textEditorThemes == null || listOfAllThemes == null)
                BuildThemeCollections();

            ThemeBase theme;
            textEditorThemes.TryGetValue(selectedThemeName, out theme);

            // Fall back to default if all else fails
            if (theme == null)
            {
                textEditorThemes.TryGetValue(DefaultThemeName, out theme);
                selectedThemeName = theme.HlThemeName;
            }

            return theme;
        }
    }

    public IList<ITheme> ListAllThemes
    {
        get
        {
            if (textEditorThemes == null || listOfAllThemes == null)
                BuildThemeCollections();

            return listOfAllThemes;
        }
    }
    #endregion properties

    public bool SetSelectedTheme(string themeName)
    {
        if (textEditorThemes == null || listOfAllThemes == null)
            BuildThemeCollections();

        ThemeBase theme;
        this.textEditorThemes.TryGetValue(themeName, out theme);

        // Fall back to default if all else fails
        if (theme == null)
            return false;

        selectedThemeName = themeName;

        return true;
    }

    private void BuildThemeCollections()
    {
        textEditorThemes = BuildThemeDictionary();
        listOfAllThemes = new ObservableCollection<ITheme>();

        foreach (var t in textEditorThemes)
        {
            listOfAllThemes.Add(t.Value);
        }
    }

    private SortedDictionary<string, ThemeBase> BuildThemeDictionary()
    {
        var themes = new SortedDictionary<string, ThemeBase>();

        try
        {
            var themeName = MetroDarkThemeName;
            var wpfTheme = new List<string>(MetroDarkResources);

            var t = new ThemeBase(this, wpfTheme, themeName);
            themes.Add(t.HlThemeName, t);
        }
        catch (System.Exception exp)
        {
            MessageDialog.Show(exp.Message);
        }

        return themes;
    }
}
