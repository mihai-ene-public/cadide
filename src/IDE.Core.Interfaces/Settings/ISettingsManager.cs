
namespace IDE.Core.Interfaces
{

    public interface ISettingsManager //: IService
    {
        IPreferences SettingData { get; }

        IProfile SessionData { get; }

        string LayoutFileName { get; }

        string AppDir { get; }

        T GetSetting<T>() where T : ISettingData;


        #region methods
        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="optionsModel"></param>
        /// <returns></returns>
        void SaveOptions(string settingsFileName);

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <returns></returns>
        void LoadOptions(string settingsFileName);

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        /// <param name="profileDataFileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        void SaveProfileData(string profileDataFileName);

        /// <summary>
        /// Save program options into persistence.
        /// </summary>
        void LoadProfileData(string profileDataFileName);

       
        #endregion methods
    }

    public interface IProfile
    {
    }

    public interface ISettingData
    {
        string Name { get; set; }
    }
}
