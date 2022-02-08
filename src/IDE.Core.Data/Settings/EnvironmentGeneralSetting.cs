using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Settings
{
    public class EnvironmentGeneralSetting : BasicSetting
    {
        string currentTheme = "Metro Dark";//ThemesManager.DefaultThemeName;

        /// <summary>
        /// Get/set WPF theme configured for the complete Application
        /// </summary>
        [XmlElement("CurrentTheme")]
        public string CurrentTheme
        {
            get
            {
                return currentTheme;
            }

            set
            {
                if (currentTheme != value)
                {
                    currentTheme = value;
                }
            }
        }

        string selectedLanguage = "en-us";

        [XmlElement("LanguageSelected")]
        public string LanguageSelected
        {
            get
            {
                return selectedLanguage;
            }

            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                }
            }
        }

        [XmlElement]
        public int NumberItemsVisibleInWindowMenu { get; set; } = 10;

        [XmlElement]
        public int NumberItemsVisibleInMRU { get; set; } = 10;

        [XmlElement]
        public bool AutoDetectFileIsChanged { get; set; } = true;

        [XmlElement]
        public bool AutoReloadFiles { get; set; } = false;

        [XmlElement]
        public bool CheckForUpdates { get; set; } = false;

    }
}
