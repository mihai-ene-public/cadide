namespace IDE.Dialogs.About
{
    using System;
    using System.Reflection;
    using Core;
    using IDE.Core.Presentation.Resources;
    using IDE.Core.ViewModels;

    public class AboutViewModel : DialogViewModel
    {
        public AboutViewModel()
            : base()
        {
        }

        public string WindowTitle
        {
            get
            {
                return AboutDialogStrings.WindowTitle;
            }
        }

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

        public string AppUrl
        {
            get
            {
                return AppHelpers.ApplicationUrl;
            }
        }

        public string AppVersion
        {
            get
            {
                return AppHelpers.ApplicationVersion;
            }
        }

        public string RunTimeVersion
        {
            get
            {
                return AppHelpers.ApplicationRuntimeVersion;
            }
        }
        
    }
}
