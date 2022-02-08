namespace IDE.Dialogs.About
{
    using System;
    using System.Reflection;
    using Core;
    using IDE.Core.Presentation.Resources;
    using IDE.Core.ViewModels;

    /// <summary>
    /// Organize the viewmodel for an about program information presentation
    /// (e.g. About dialog)
    /// </summary>
    public class AboutViewModel : DialogViewModel
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public AboutViewModel()
            : base()
        {
            licenseStatus = new LicenseStatus();
        }

        #endregion constructor

        LicenseStatus licenseStatus;

        /// <summary>
        /// Get the title string of the view - to be displayed in the associated view
        /// (e.g. as dialog title)
        /// </summary>
        public string WindowTitle
        {
            get
            {
                //return $"About {AppHelpers.ApplicationTitle}";
                return AboutDialogStrings.WindowTitle;
            }
        }



        /// <summary>
        /// Get title of application for display in About view.
        /// </summary>
        public string AppTitle
        {
            get
            {
                return AppHelpers.ApplicationFullTitle;
            }
        }

        public string SubTitle
        {
            get
            {
                return AboutDialogStrings.SubTitle;
            }
        }

        /// <summary>
        /// Gets the assembly copyright.
        /// </summary>
        /// <value>The assembly copyright.</value>
        public string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return string.Empty;

                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Get URL of application for reference of source and display in About view.
        /// </summary>
        public string AppUrl
        {
            get
            {
                return AppHelpers.ApplicationUrl;
            }
        }

       

        /// <summary>
        /// Get application version for display in About view.
        /// </summary>
        public string AppVersion
        {
            get
            {
                return AppHelpers.ApplicationVersion;
            }
        }

        /// <summary>
        /// Get version of runtime for display in About view.
        /// </summary>
        public string RunTimeVersion
        {
            get
            {
                return AppHelpers.ApplicationRuntimeVersion;
            }
        }

        public string LicenseStatus
        {
            get
            {
                try
                {
                    return licenseStatus.GetLicenseStatusString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
