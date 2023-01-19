namespace IDE.Core.ViewModels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    public class AppHelpers
    {

        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public static string DirAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XNoCAD");


        public static string AssemblyTitle => Assembly.GetEntryAssembly().GetName().Name;

        public static string ApplicationTitle => "XNoCAD";

        public static string ApplicationFullTitle => $"{ApplicationTitle}";

        public static string ApplicationUrl => "https://github.com/mihai-ene-public/cadide";

        public static string ApplicationVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
        public static string ApplicationRuntimeVersion => Assembly.GetEntryAssembly().ImageRuntimeVersion;

        public static int ApplicationVersionMajor => Assembly.GetEntryAssembly().GetName().Version.Major;

        public static string AssemblyEntryLocation => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);


        public static string LayoutFileName => "Layout.config";

        public static string SolutionConfigFolderName => ".ide";

        public static string DirFileAppSessionData => Path.Combine(DirAppData, string.Format(CultureInfo.InvariantCulture, "{0}.App.session", AssemblyTitle));

        public static string DirFileAppSettingsData
        {
            get
            {
                return System.IO.Path.Combine(DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
                                                             AssemblyTitle));
            }
        }

    }
}
