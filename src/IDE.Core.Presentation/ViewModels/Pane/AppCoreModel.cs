namespace IDE.Core.ViewModels
{
    using IDE.Core.Interfaces;
    using System;

    public class AppCoreModel : IAppCoreModel
    {
        /// <summary>
        /// Gets the file name of the layout file that is useful for AvalonDock.
        /// </summary>
        public string LayoutFileName
        {
            get
            {
                return "Layout.config";
            }
        }

        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public string DirAppData
        {
            get
            {
                return AppHelpers.DirAppData;
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public string MyDocumentsUserDir
        {
            get
            {
                return AppHelpers.MyDocumentsUserDir;
            }
        }

        /// <summary>
        /// Get the name of the executing assembly (usually name of *.exe file)
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                return AppHelpers.AssemblyTitle;
            }
        }

        public string ApplicationTitle
        {
            get { return AppHelpers.ApplicationTitle; }
        }

        //
        // Summary:
        //     Gets the path or UNC location of the loaded file that contains the manifest.
        //
        // Returns:
        //     The location of the loaded file that contains the manifest. If the loaded
        //     file was shadow-copied, the location is that of the file after being shadow-copied.
        //     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
        //     method overload, the value returned is an empty string ("").
        public string AssemblyEntryLocation
        {
            get
            {
                return AppHelpers.AssemblyEntryLocation;
            }
        }


        /// <summary>
        /// Get path and file name to application specific session file
        /// </summary>
        public string DirFileAppSessionData => AppHelpers.DirFileAppSessionData;

        /// <summary>
        /// Get path and file name to application specific settings file
        /// </summary>
        public string DirFileAppSettingsData
        {
            get
            {
                return AppHelpers.DirFileAppSettingsData;
            }
        }

        

    }
}
