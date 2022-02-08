namespace IDE.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Serialization;
    using IDE.Core.Interfaces;

    /// <summary>
    /// Determine whether Zoom units of the text editor
    /// are displayed in percent or font related points.
    /// </summary>
    public enum ZoomUnit
    {
        Percentage = 0,
        Points = 1
    }

    [Serializable]
    [XmlRoot("Options")]
    public class Preferences : IPreferences
    {
        public Preferences()
        {
        }

        [XmlElement("Category", typeof(SettingCategory))]
        public List<BasicSettingNode> Settings { get; set; } = new List<BasicSettingNode>();

        [XmlIgnore]
        IList<ISettingCategoryData> IPreferences.Settings { get => Settings.Cast<ISettingCategoryData>().ToList(); }


    }
}
