namespace IDE.Core.Themes
{
    using IDE.Core.Interfaces;
    using System.Collections.Generic;

    public class ThemeBase : ITheme
    {
        #region fields

        private IParentSelectedTheme parent = null;
       
        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        internal ThemeBase(IParentSelectedTheme parent,
                           List<string> resources,
                           string wpfThemeName
                           )
        {
            this.parent = parent;
            Resources = new List<string>(resources);
            WPFThemeName = wpfThemeName;
        }

       
        #endregion constructor

        #region properties
        /// <summary>
        /// Get a list of Uri formatted resource strings that point to all relevant resources.
        /// </summary>
        public List<string> Resources { get; private set; }

        /// <summary>
        /// WPF Application skin theme (e.g. Metro)
        /// </summary>
        public string WPFThemeName { get; private set; }


        /// <summary>
        /// Get the human read-able name of this WPF/Editor theme.
        /// </summary>
        public string HlThemeName
        {
            get
            {
                return WPFThemeName;
                //return string.Format("{0}{1}", WPFThemeName,
                //                            EditorThemeName == null ? string.Empty : " (" + EditorThemeName + ")");
            }
        }

        /// <summary>
        /// Determine whether this theme is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (parent != null)
                {
                    if (parent.SelectedThemeName != null)
                    {
                        return parent.SelectedThemeName.Equals(HlThemeName);
                    }
                }
                return false;
            }
        }
        #endregion properties
    }
}
