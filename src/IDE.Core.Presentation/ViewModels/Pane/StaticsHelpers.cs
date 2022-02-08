namespace IDE.Core.ViewModels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Class maintains and helps access to core facts of this application.
    /// Core facts are installation directory, name of application etc.
    /// 
    /// This class should not be used directly unless it is realy necessary.
    /// Use the <seealso cref="AppCoreModel"/> through its interface and
    /// constructor dependency injection to avoid unnecessary dependencies
    /// and problems when refactoring later on.
    /// </summary>
    public class AppHelpers
    {

        #region properties
        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public static string DirAppData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                System.IO.Path.DirectorySeparatorChar +
                                                Company;
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public static string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public static string LicenseFilePath => Path.Combine(DirAppData, $"license.license");

        /// <summary>
        /// Get the name of the executing assembly (usually name of *.exe file)
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        public static string ApplicationTitle
        {
            get { return "Modern PCB Designer"; }
        }

        /// <summary>
        /// contains edition name
        /// </summary>
        public static string ApplicationFullTitle => $"{ApplicationTitle}";

        public static string ApplicationUrl => "https://modernpcbstudio.com/";

        public static string ApplicationVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
        public static string ApplicationRuntimeVersion => Assembly.GetEntryAssembly().ImageRuntimeVersion;

        public static int ApplicationVersionMajor => Assembly.GetEntryAssembly().GetName().Version.Major;

        //
        // Summary:
        //     Gets the path or UNC location of the loaded file that contains the manifest.
        //
        // Returns:
        //     The location of the loaded file that contains the manifest. If the loaded
        //     file was shadow-copied, the location is that of the file after being shadow-copied.
        //     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
        //     method overload, the value returned is an empty string ("").
        public static string AssemblyEntryLocation
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        public static string Company
        {
            get
            {
                return "modernpcbstudio.com";
            }
        }

        public static string LayoutFileName => "Layout.config";

        /// <summary>
        /// .ide folder
        /// </summary>
        public static string SolutionConfigFolderName => ".ide";

        /// <summary>
        /// Get path and file name to application specific session file
        /// </summary>
        public static string DirFileAppSessionData
        {
            get
            {
                return System.IO.Path.Combine(DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.session", AssemblyTitle));
            }

        }

        /// <summary>
        /// Get path and file name to application specific settings file
        /// </summary>
        public static string DirFileAppSettingsData
        {
            get
            {
                return System.IO.Path.Combine(DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
                                                             AssemblyTitle));
            }
        }
        #endregion properties

    }
}
