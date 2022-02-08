namespace IDE.Core.Interfaces
{
    using System.Collections.Generic;

    public interface IPreferences
    {
        IList<ISettingCategoryData> Settings { get; }// set; }

        //string CurrentTheme { get; }

        //string LanguageSelected { get; }


    }

    public interface ISettingCategoryData : ISettingData
    {
        IList<ISettingData> Children { get; }
    }
}
