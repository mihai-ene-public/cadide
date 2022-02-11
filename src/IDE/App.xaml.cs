using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.App.Views.Shell;
using System.Reflection;
using System.IO;

namespace IDE
{

    public partial class MainApp : Application
    {

        #region constructor
        public MainApp()
        {
            AppIsShuttingDown = false;

            Startup += Application_Startup;
            SessionEnding += App_SessionEnding;
            DispatcherUnhandledException += MainApp_DispatcherUnhandledException;
        }


        //called when OS logs-out or shuts-down
        void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            AppIsShuttingDown = true;
        }
        #endregion constructor

        public bool AppIsShuttingDown { get; set; }

        #region methods
        /// <summary>
        /// Check if end of application session should be canceled or not
        /// (we may have gotten here through unhandled exceptions - so we
        /// display it and attempt CONTINUE so user can save his data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);

            try
            {
                var appVM = GetWorkSpace();

                if (appVM != null && appVM.Files != null)
                {
                    // Close all open files and check whether application is ready to close
                    if (appVM.CanCloseAndSaved() == true)
                        e.Cancel = false;
                    else
                        e.Cancel = appVM.ShutDownInProgress_Cancel = true;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// This is the first bit of code being executed when the application is invoked (main entry point).
        /// 
        /// Use the <paramref name="e"/> parameter to evaluate command line options.
        /// Invoking a program with an associated file type extension (eg.: *.txt) in Explorer
        /// results, for example, in executing this function with the path and filename being
        /// supplied in <paramref name="e"/>.
        /// </summary>
        void Application_Startup(object sender, StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.PreviewMouseRightButtonDownEvent, new RoutedEventHandler(TreeViewItem_PreviewMouseRightButtonDownEvent));

            try
            {
                // Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
                // durring start-up without shutting down application when the custom dialogs (messagebox) closes
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch
            {
            }

            try
            {
                IDE.Startup.Run(e);
            }
            catch (Exception exp)
            {
                // Logger.Error(exp);

                // Cannot set shutdown mode when application is already shuttong down
                if (AppIsShuttingDown == false)
                    ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // 1) Application hangs when this is set to null while MainWindow is visible
                // 2) Application throws exception when this is set as owner of window when it
                //    was never visible.
                //
                if (Application.Current.MainWindow != null)
                {
                    if (Application.Current.MainWindow.IsVisible == false)
                        Application.Current.MainWindow = null;
                }

                MessageDialog.Show(exp.Message);

                if (AppIsShuttingDown == false)
                    Shutdown();
            }

        }

        //this behavior is intended to be for all TreeViewItems: on right click, select the node
        void TreeViewItem_PreviewMouseRightButtonDownEvent(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem tvi)
                tvi.IsSelected = true;
        }

        private void MainApp_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                MessageDialog.Show(e.Exception.Message);

                e.Handled = true;
            }
            catch
            {
            }
        }

        private IApplicationViewModel GetWorkSpace()
        {
            return MainWindow.DataContext as IApplicationViewModel;
        }
        #endregion methods
    }
}
