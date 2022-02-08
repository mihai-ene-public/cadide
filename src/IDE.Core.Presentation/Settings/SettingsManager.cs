namespace IDE.Core.Settings
{
    using IDE.Core.Interfaces;
    using IDE.Core.Types.Input;
    using IDE.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// This class keeps track of program options and user profile (session) data.
    /// Both data items can be added and are loaded on application start to restore
    /// the program state of the last user session or to implement the default
    /// application state when starting the application for the very first time.
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        public const string DefaultTheme = "Metro Dark";

        public const string DefaultLocal = "en-US";

        #region constructor

        public SettingsManager()
        {
            SettingData = new Preferences();
            SessionData = new Profile();
        }

        #endregion constructor

        #region fields

        IPreferences preferencesData = null;
        IProfile profileData = null;


        #endregion fields

        #region properties
        /// <summary>
        /// Gets the program options for the complete application.
        /// Program options are user specific settings that are rarelly
        /// changed and can be customized per user.
        /// </summary>
        public IPreferences SettingData
        {
            get
            {
                if (preferencesData == null)
                    preferencesData = new Preferences();

                return preferencesData;
            }

            private set
            {
                if (preferencesData != value)
                    preferencesData = value;
            }
        }

        /// <summary>
        /// Gets the user session data that is almost always bound to change
        /// everytime the user starts up the application, does something with it,
        /// and shuts it down. Typically, window position, recent files list,
        /// and such things are stored in here.
        /// </summary>
        public IProfile SessionData
        {
            get
            {
                if (profileData == null)
                    profileData = new Profile();

                return profileData;
            }

            private set
            {
                if (profileData != value)
                    profileData = value;
            }
        }

        public string LayoutFileName => AppHelpers.LayoutFileName;

        public string AppDir => AppHelpers.DirAppData;
        #endregion properties

        #region methods
        

        #region Load Save ProgramOptions
        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        public void LoadOptions(string settingsFileName)
        {
            Preferences loadedModel = LoadPreferences(settingsFileName);

            SettingData = loadedModel;

            CreateLinearListSettings();
        }

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        public static Preferences LoadPreferences(string settingsFileName)
        {
            Preferences loadedModel = null;

            try
            {
                if (File.Exists(settingsFileName))
                {
                    loadedModel = XmlHelper.Load<Preferences>(settingsFileName);
                }
            }
            catch
            {
            }
            finally
            {
                // Just get the defaults if serilization wasn't working here...
                if (loadedModel == null)
                    loadedModel = new Preferences();
            }

            return loadedModel;
        }

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="optionsModel"></param>
        /// <returns></returns>
        public void SaveOptions(string settingsFileName)//, Preferences optionsModel)
        {
            try
            {
                //clear cache
                settingsLinear.Clear();
                XmlHelper.Save(SettingData, settingsFileName);
            }
            catch
            {
                throw;
            }
        }
        #endregion Load Save ProgramOptions

        #region Load Save UserSessionData
        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <returns></returns>
        public void LoadProfileData(string profileDataFileName)
        {
            Profile profileDataModel = null;

            try
            {
                if (File.Exists(profileDataFileName))
                {
                    profileDataModel = XmlHelper.Load<Profile>(profileDataFileName);
                }

                SessionData = profileDataModel;
            }
            catch //(Exception exp)
            {
                //logger.Error(exp);
            }
            finally
            {
                if (profileDataModel == null)
                    profileDataModel = new Profile();  // Just get the defaults if serilization wasn't working here...
            }
        }

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="profileDataFileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public void SaveProfileData(string profileDataFileName)
        {
            try
            {
                XmlHelper.Save(profileData, profileDataFileName);
            }
            catch
            {
                throw;
            }
        }

        #endregion Load Save UserSessionData
        #endregion methods

        void CreateDefaultSettings()
        {
            if (SettingData == null)
                return;

            var sd = SettingData as Preferences;

            if (SettingData.Settings == null || SettingData.Settings.Count == 0)
            {


                sd.Settings = new List<BasicSettingNode>
                            {
                                new SettingCategory
                                {
                                     Name="Environment",
                                     Children=new List<BasicSetting>
                                     {
                                          new EnvironmentGeneralSetting
                                          {
                                              Name="General"
                                          },
                                          new EnvironmentKeyboardSetting
                                          {
                                              Name="Keyboard",
                                               KeySettings=new List<KeyboardSetting>()
                                               {
                                                    new KeyboardSetting{ Operation = KeyboardOperations.Rotate, Modifiers = XModifierKeys.None, Key = XKey.Space },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.MirrorX, Modifiers=XModifierKeys.None, Key= XKey.X },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.MirrorY, Modifiers=XModifierKeys.None, Key= XKey.Y },

                                                    new KeyboardSetting{ Operation = KeyboardOperations.Copy, Modifiers=XModifierKeys.Control, Key= XKey.C },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.Paste, Modifiers=XModifierKeys.Control, Key= XKey.V },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.Cut, Modifiers=XModifierKeys.Control, Key= XKey.X },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.Delete, Modifiers=XModifierKeys.None, Key = XKey.Delete },

                                                    new KeyboardSetting{ Operation = KeyboardOperations.Undo, Modifiers=XModifierKeys.Control, Key = XKey.Z },
                                                    new KeyboardSetting{ Operation = KeyboardOperations.Redo, Modifiers=XModifierKeys.Control, Key = XKey.Y },
                                               }
                                          },
                                           new EnvironmentFolderLibsSettingData
                                           {
                                               Name="Folders"
                                           },
                                     }
                                },
                                new SettingCategory
                                {
                                    Name="Schematic",
                                    Children=new List<BasicSetting>
                                    {
                                        new SchematicEditorColorsSetting
                                        {
                                            Name="Colors"
                                        },
                                        new SchematicEditorPrimitiveDefaults(true)
                                        {
                                            Name="Primitives"
                                        }
                                    }
                                },
                                new SettingCategory
                                {
                                    Name="Board",
                                    Children=new List<BasicSetting>
                                    {
                                         new BoardEditorGeneralSetting
                                        {
                                             Name="General"
                                        },
                                        new BoardEditorColorsSetting
                                        {
                                            Name="Colors"
                                        },
                                         new BoardEditorRoutingSetting
                                        {
                                             Name="Routing"
                                        },
                                        new BoardEditorPrimitiveDefaults(true)
                                        {
                                            Name="Primitives"
                                        }
                                    }
                                }
                            };
            }

            var storeSettings = sd.Settings;

            //component category
            var componentCategory = storeSettings.FirstOrDefault(s => s.Name == "Component") as SettingCategory;
            if (componentCategory == null)
            {
                componentCategory = new SettingCategory { Name = "Component" };
                storeSettings.Add(componentCategory);
            }
            ////component BOM setting
            //var compBomSettingName = "Component Editor BOM";
            //var compBomSetting = componentCategory.Children.FirstOrDefault(s => s.Name == compBomSettingName);
            //if (compBomSetting == null)
            //{
            //    compBomSetting = new ComponentEditorBOMSetting
            //    {
            //        Name = compBomSettingName,
            //        Suppliers = new List<string>(new[] { "Digi-Key", "Mouser", "Farnell" })
            //    };
            //    componentCategory.Children.Add(compBomSetting);
            //}
        }

        List<BasicSetting> settingsLinear = new List<BasicSetting>();

        void CreateLinearListSettings()
        {
            settingsLinear.Clear();

            CreateDefaultSettings();//created as needed

            foreach (var cat in SettingData.Settings)
            {
                foreach (var c in cat.Children)
                {
                    if (c is BasicSetting)
                        settingsLinear.Add((BasicSetting)c);
                }
            }
        }

        public T GetSetting<T>() where T : ISettingData
        {
            if (settingsLinear.Count == 0)
                CreateLinearListSettings();

            var s = settingsLinear.OfType<T>().FirstOrDefault();
            return s;
        }
    }
}
