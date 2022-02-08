namespace IDE.Controls
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Documents;

    using System;
    using System.Runtime.InteropServices;
using IDE.Core.Utilities;

    public partial class WebHyperlink : UserControl
    {
        #region fields

        private static readonly DependencyProperty NavigateUriProperty =
          DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(WebHyperlink));

        private static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string), typeof(WebHyperlink));

        private static RoutedCommand copyUriCommand;
        private static RoutedCommand navigateToUriCommand;

        private System.Windows.Documents.Hyperlink hyperLink;

        #endregion fields

        #region constructor

        static WebHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WebHyperlink),
                      new FrameworkPropertyMetadata(typeof(WebHyperlink)));

            WebHyperlink.copyUriCommand = new RoutedCommand("CopyUri", typeof(WebHyperlink));

            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(copyUriCommand, CopyHyperlinkUri));
            CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(copyUriCommand, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            WebHyperlink.navigateToUriCommand = new RoutedCommand("NavigateToUri", typeof(WebHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(navigateToUriCommand, Hyperlink_CommandNavigateTo));
            ////CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));
        }

        public WebHyperlink()
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

        /// <summary>
        /// Declare NavigateUri property to allow a user who clicked
        /// on the dispalyed Hyperlink to navigate their with their installed browser...
        /// </summary>
        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            hyperLink = GetTemplateChild("PART_Hyperlink") as Hyperlink;
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

            var whLink = sender as WebHyperlink;

            if (whLink == null) return;

            try
            {
                //Process.Start(new ProcessStartInfo(whLink.NavigateUri.AbsoluteUri));
                ProcessStarter.Start(whLink.NavigateUri.AbsoluteUri);
            }
            catch //(System.Exception ex)
            {
                //Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}.", ex.Message),
                //         Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                //         MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageDialog.Show(ex.Message);
            }
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

            var whLink = sender as WebHyperlink;

            if (whLink == null) return;

            try
            {
                Clipboard.SetText(whLink.NavigateUri.AbsoluteUri);
            }
            catch
            {
                Clipboard.SetText(whLink.NavigateUri.OriginalString);
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var url = e.Uri.AbsoluteUri;
            try
            {
                //var psi = new ProcessStartInfo
                //{
                //    FileName = url,
                //    UseShellExecute = true
                //};
                //Process.Start(psi);
                ProcessStarter.Start(url);
            }
            catch //(System.Exception ex)
            {
                //// hack because of this: https://github.com/dotnet/corefx/issues/10361
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //{
                //    url = url.Replace("&", "^&");
                //    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                //{
                //    Process.Start("xdg-open", url);
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                //{
                //    Process.Start("open", url);
                //}
                //else
                //{
                //    throw;
                //}
            }
        }
        #endregion
    }
}
