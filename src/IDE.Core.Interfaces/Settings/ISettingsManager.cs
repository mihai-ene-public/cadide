
namespace IDE.Core.Interfaces
{

    public interface ISettingsManager
    {
        IPreferences SettingData { get; }

        IProfile SessionData { get; }

        string LayoutFileName { get; }

        string AppDir { get; }

        T GetSetting<T>() where T : ISettingData;

        void SaveOptions(string settingsFileName);

        void LoadOptions(string settingsFileName);

        void SaveProfileData(string profileDataFileName);

        void LoadProfileData(string profileDataFileName);

    }
}
