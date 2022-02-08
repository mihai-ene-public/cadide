namespace IDE.Core.Settings
{
    using IDE.Core.Interfaces;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// This class implements the model of the user profile part
    /// of the application. Typically, users have implicit run-time
    /// settings that should be re-activated when the application
    /// is re-started at a later point in time (e.g. window size
    /// and position).
    /// 
    /// This class organizes these per user specific profile settings
    /// and is responsible for their storage (at program end) and
    /// retrieval at the start-up of the application.
    /// </summary>
    public class Profile : IProfile
    {

        public Profile()
        {
        }

        /// <summary>
        /// List of most recently used files
        /// </summary>
        [XmlArray("mruList")]
        [XmlArrayItem("mru")]
        public List<MruItem> MruList { get; set; } = new List<MruItem>();


    }
}
