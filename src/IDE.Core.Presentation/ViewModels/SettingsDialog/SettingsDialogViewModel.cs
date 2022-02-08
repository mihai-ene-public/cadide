using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IDE.Core;
using IDE.Core.Settings;
using IDE.Core.Settings.Options;
using IDE.Core.Interfaces;
using IDE.Core.Common;
using IDE.Core.Presentation.Resources;
using System.Windows.Input;

namespace IDE.Documents.Views
{



    public class SettingsDialogViewModel : DialogViewModel
    {


        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public SettingsDialogViewModel()
            : base()
        {

            // Initialize localization settings
            Languages = new List<LanguageFamily>() { new LanguageFamily() { Language = "en", Locale = "US", Name = "English" } };//Preferences.GetSupportedLanguages());

            // Set default language to make sure app neutral is selected and available for sure
            // (this is a fallback if all else fails)
            try
            {
                LanguageSelected = Languages.FirstOrDefault(lang => lang.BCP47 == SettingsManager.DefaultLocal);
            }
            catch
            {
            }
        }
        #endregion constructor

        #region properties


        #region ScaleView

        /// <summary>
        /// Get the title string of the view - to be displayed in the associated view
        /// (e.g. as dialog title)
        /// </summary>
        public string WindowTitle
        {
            get
            {
                return Strings.STR_ProgramSettings_Caption;
            }
        }

        #endregion ScaleView

        #region Language Localization Support
        /// <summary>
        /// Get list of GUI languages supported in this application.
        /// </summary>
        public List<LanguageFamily> Languages { get; private set; }

        LanguageFamily languageSelected;

        /// <summary>
        /// Get/set language of message box buttons for display in localized form.
        /// </summary>
        public LanguageFamily LanguageSelected
        {
            get
            {
                return languageSelected;
            }

            set
            {
                if (languageSelected != value)
                {
                    languageSelected = value;
                    OnPropertyChanged(nameof(LanguageSelected));
                }
            }
        }

        #endregion Language Localization Support


        public ObservableCollection<BasicSettingModel> Settings { get; set; } = new ObservableCollection<BasicSettingModel>();

        BasicSettingModel currentSetting;

        public BasicSettingModel CurrentSetting
        {
            get
            {
                return currentSetting;
            }
            set
            {
                currentSetting = value;
                OnPropertyChanged(nameof(CurrentSetting));
            }
        }

        #endregion properties

        ICommand resetCurrentSettingCommand;

        public ICommand ResetCurrentSettingCommand
        {
            get
            {
                if (resetCurrentSettingCommand == null)
                    resetCurrentSettingCommand = CreateCommand(p => CurrentSetting?.ResetSetting());

                return resetCurrentSettingCommand;
            }
        }

        ICommand resetAllSettingsCommand;

        public ICommand ResetAllSettingsCommand
        {
            get
            {
                if (resetAllSettingsCommand == null)
                    resetAllSettingsCommand = CreateCommand(p =>
                   {
                       foreach (var s in Settings)
                           s.ResetSetting();
                   });

                return resetAllSettingsCommand;
            }
        }

        #region methods

        IEnumerable<BasicSettingModel> allSettings;

        /// <summary>
        /// Reset the view model to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="settingData"></param>
        public void LoadOptionsFromData(IPreferences settingData)
        {


            LanguageSelected = Languages.FirstOrDefault();

            Settings.Clear();

            if (settingData.Settings != null)
            {
                allSettings = settingData.Settings.Select(s => (BasicSettingModel)s.CreateModelItem())
                    .Where(s => s != null)
                    .ToList();

                //   Settings.AddRange(settingData.Settings.Select(s => (BasicSettingNodeModel)s.CreateModelItem()));
                //var dataSettings = settingData.Settings.OfType<SettingCategory>()
                //                                       .SelectMany(s => s.Children)
                //                                       .Select(s => (BasicSettingModel)s.CreateModelItem());

                var dataSettings = allSettings.OfType<SettingCategoryModel>()
                                              .SelectMany(s => s.Children)
                                              .Cast<BasicSettingModel>();

                Settings.AddRange(dataSettings);

                CurrentSetting = Settings.Cast<BasicSettingModel>().FirstOrDefault(s => s.IsVisible);
            }
        }

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        public void SaveOptionsToData(IPreferences settingData)
        {
            ((Preferences)settingData).Settings = allSettings.Select(s => (BasicSettingNode)s.ToData()).ToList();
        }


        #endregion methods
    }
}
