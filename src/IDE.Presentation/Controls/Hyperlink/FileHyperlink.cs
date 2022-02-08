namespace IDE.Controls
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using IDE.Controls;

    using System.IO;
    using System;
    using IDE.Core.Utilities;

    public partial class FileHyperlink : UserControl
    {
        #region fields
        private static readonly DependencyProperty NavigateUriProperty =
          DependencyProperty.Register("NavigateUri", typeof(string), typeof(FileHyperlink));

        private static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string), typeof(FileHyperlink));

        private static RoutedCommand copyUriCommand;
        private static RoutedCommand navigateToUriCommand;
        private static RoutedCommand openContainingFolderCommand;

        private System.Windows.Documents.Hyperlink hyperLink;

        #endregion fields

        #region constructor
        static FileHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileHyperlink),
                      new FrameworkPropertyMetadata(typeof(FileHyperlink)));

            FileHyperlink.copyUriCommand = new RoutedCommand("CopyUri", typeof(FileHyperlink));

            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(copyUriCommand, CopyHyperlinkUri));
            CommandManager.RegisterClassInputBinding(typeof(FileHyperlink), new InputBinding(copyUriCommand, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            FileHyperlink.navigateToUriCommand = new RoutedCommand("NavigateToUri", typeof(FileHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(navigateToUriCommand, Hyperlink_CommandNavigateTo));
            ////CommandManager.RegisterClassInputBinding(typeof(FileHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            FileHyperlink.openContainingFolderCommand = new RoutedCommand("OpenContainingFolder", typeof(FileHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(openContainingFolderCommand, Hyperlink_OpenContainingFolder));
        }

        public FileHyperlink()
        {
            hyperLink = null;
        }
        #endregion constructor

        #region properties
        public static RoutedCommand CopyUri
        {
            get
            {
                return copyUriCommand;
            }
        }

        public static RoutedCommand NavigateToUri
        {
            get
            {
                return navigateToUriCommand;
            }
        }

        public static RoutedCommand OpenContainingFolder
        {
            get
            {
                return openContainingFolderCommand;
            }
        }

        /// <summary>
        /// Declare NavigateUri property to allow a user who clicked
        /// on the dispalyed Hyperlink to navigate their with their installed browser...
        /// </summary>
        public string NavigateUri
        {
            get { return (string)GetValue(FileHyperlink.NavigateUriProperty); }
            set { SetValue(FileHyperlink.NavigateUriProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(FileHyperlink.TextProperty); }
            set { SetValue(FileHyperlink.TextProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// </summary>
        /// <param name="oFileName"></param>
        /// <returns></returns>
        public static bool OpenFileLocationInWindowsExplorer(object oFileName)
        {
            string sFileName = oFileName as string;

            if ((sFileName == null ? string.Empty : sFileName).Length == 0) return true;

            try
            {
                if (File.Exists(sFileName))
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    var argument = @"/select, " + sFileName;

                    Process.Start("explorer.exe", argument);
                    return true;
                }
                else
                {
                    var sParentDir = Directory.GetParent(sFileName).FullName;

                    if (Directory.Exists(sParentDir) == false)
                    {
                        //Msg.Show(string.Format(Strings.STR_MSG_DIRECTORY_DOES_NOT_EXIST, sParentDir),
                        //     Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                        //         MessageBoxButton.OK, MessageBoxImage.Error);
                        //MessageDialog.Show(string.Format(Strings.STR_MSG_DIRECTORY_DOES_NOT_EXIST, sParentDir));
                    }
                    else
                    {
                        // combine the arguments together it doesn't matter if there is a space after ','
                        string argument = @"/select, " + sParentDir;

                        Process.Start("explorer.exe", argument);

                        return true;
                    }
                }
            }
            catch// (System.Exception ex)
            {
                //Msg.Show(string.Format("{0}\n'{1}'.", ex.Message, (sFileName == null ? string.Empty : sFileName)),
                //          Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                //          MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageDialog.Show(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// executed when control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            hyperLink = GetTemplateChild("PART_Hyperlink") as System.Windows.Documents.Hyperlink;
            Debug.Assert(hyperLink != null, "No Hyperlink in ControlTemplate!");

            // Attach hyperlink event clicked event handler to Hyperlink ControlTemplate if there is no command defined
            // Commanding allows calling commands that are external to the control (application commands) with parameters
            // that can differ from whats available in this control (using converters and what not)
            //
            // Therefore, commanding overrules the Hyperlink.Clicked event when it is defined.
            if (hyperLink != null)
            {
                if (hyperLink.Command == null)
                    hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
            }
        }

        /// <summary>
        /// Process command when a hyperlink has been clicked.
        /// Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Hyperlink_CommandNavigateTo(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender == null || e == null) return;

            e.Handled = true;

            var whLink = sender as FileHyperlink;

            if (whLink == null) return;

            try
            {
                //Process.Start(new ProcessStartInfo(whLink.NavigateUri));
                ProcessStarter.Start(whLink.NavigateUri);
                ////OpenFileLocationInWindowsExplorer(whLink.NavigateUri.OriginalString);
            }
            catch //(Exception ex)
            {
                //Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (whLink.NavigateUri == null ? string.Empty : whLink.NavigateUri.ToString())),
                //         Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                //         MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageDialog.Show(ex.Message);
            }
        }

        private static void Hyperlink_OpenContainingFolder(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender == null || e == null) return;

            e.Handled = true;

            var whLink = sender as FileHyperlink;

            if (whLink == null) return;

            OpenFileLocationInWindowsExplorer(whLink.NavigateUri);
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CopyHyperlinkUri(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender == null || e == null) return;

            e.Handled = true;

            var whLink = sender as FileHyperlink;

            if (whLink == null) return;

            try
            {
                Clipboard.SetText(whLink.NavigateUri);
            }
            catch
            {
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                //Process.Start(new ProcessStartInfo(this.NavigateUri));
                ProcessStarter.Start(this.NavigateUri);
            }
            catch //(System.Exception ex)
            {
                //Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (this.NavigateUri == null ? string.Empty : this.NavigateUri.ToString())),
                //         Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                //         MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageDialog.Show(ex.Message);
            }
        }
        #endregion
    }
}
