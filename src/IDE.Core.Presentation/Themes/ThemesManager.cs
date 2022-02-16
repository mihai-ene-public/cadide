namespace IDE.Core.Themes
{
    using IDE.Core.Interfaces;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Utilities;

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


        #region fields
        #region WPF Themes

        #region Expression Dark theme resources


        static readonly string[] MetroDarkResources =
    {
      "/IDE.Presentation;component/_Themes/MetroDark/Theme.xaml",
    };
        #endregion Expression Dark theme resources


    
        #endregion WPF Themes


        public const string DefaultThemeName = MetroDarkThemeName;

        private SortedDictionary<string, ThemeBase> textEditorThemes = null;
        private IList<ITheme> listOfAllThemes = null;
       
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public ThemesManager()
        {
        }
        #endregion constructor

        #region properties

        private string selectedThemeName = DefaultThemeName;
        /// <summary>
        /// Get the name of the currently seelcted theme.
        /// </summary>
        public string SelectedThemeName
        {
            get
            {
                return selectedThemeName;
            }
        }

        /// <summary>
        /// Get the object that has links to all resources for the currently selected WPF theme.
        /// </summary>
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

        /// <summary>
        /// Get a list of all available themes (This property can typically be used to bind
        /// menuitems or other resources to let the user select a theme in the user interface).
        /// </summary>
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

        #region methods
        /// <summary>
        /// Change the WPF/EditorHighlightingTheme to the <paramref name="themeName"/> theme.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns>True if new theme is succesfully selected (was available), otherwise false</returns>
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

        /// <summary>
        /// Build sorted dictionary and observable collection for WPF themes.
        /// </summary>
        private void BuildThemeCollections()
        {
            textEditorThemes = BuildThemeDictionary();
            listOfAllThemes = new ObservableCollection<ITheme>();

            foreach (var t in textEditorThemes)
            {
                listOfAllThemes.Add(t.Value);
            }
        }

        /// <summary>
        /// Build a sorted structure of all default themes and their resources.
        /// </summary>
        /// <returns></returns>
        private SortedDictionary<string, ThemeBase> BuildThemeDictionary()
        {
            var ret = new SortedDictionary<string, ThemeBase>();

            ThemeBase t = null;
            string themeName = null;
            List<string> wpfTheme = null;

            try
            {
                themeName = MetroDarkThemeName;
                wpfTheme = new List<string>(MetroDarkResources);

                t = new ThemeBase(this, wpfTheme, themeName);
                ret.Add(t.HlThemeName, t);
            }
            catch (System.Exception exp)
            {
                MessageDialog.Show(exp.Message);
            }

            return ret;
        }
        #endregion methods
    }
}
