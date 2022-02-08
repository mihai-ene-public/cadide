using IDE.App.Views.Shell;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Licensing;
using IDE.Core.Settings;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.Documents.Module;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace IDE
{
    class Bootstrapper
    {
        public Bootstrapper(IApplicationViewModel app)
        {
            application = app;
        }

        IApplicationViewModel application;

        public void Run(StartupEventArgs eventArgs)
        {



            var settings = application.SettingsManager;

            settings.LoadOptions(AppHelpers.DirFileAppSettingsData);
            var options = settings.SettingData;

            var envSetting = settings.GetSetting<EnvironmentGeneralSetting>();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(envSetting.LanguageSelected);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(envSetting.LanguageSelected);

            ApplicationInitializer.Initialize();

            CreateShell();

            var licenseManager = new LicenseManager();
            licenseManager.VerifyLicensing();

            // Show the startpage if application starts for the very first time
            // (This requires that command binding was succesfully done before this line)
            //if (application.ADLayout.LayoutSoure == LayoutLoaded.FromDefault)
            Core.Commands.AppCommand.ShowStartPage.Execute(null);

            if (eventArgs != null)
                ProcessCmdLine(eventArgs.Args);
        }

        void CreateShell()
        {
            var mainApp = Application.Current as MainApp;
            try
            {

                ConstructMainWindowSession();




                // Register imported tool window definitions with Avalondock
                var toolWindowRegistry = ApplicationServices.ToolRegistry;
                toolWindowRegistry.PublishTools();
            }
            catch (Exception exp)
            {
                try
                {
                    // Cannot set shutdown mode when application is already shuttong down
                    if (mainApp.AppIsShuttingDown == false)
                        mainApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                }
                catch// (Exception exp1)
                {
                    // logger.Error(exp1);
                }

                try
                {
                    // 1) Application hangs when this is set to null while MainWindow is visible
                    // 2) Application throws exception when this is set as owner of window when it
                    //    was never visible.
                    //
                    if (Application.Current.MainWindow != null)
                    {
                        if (Application.Current.MainWindow.IsVisible == false)
                            Application.Current.MainWindow = null;
                    }
                }
                catch// (Exception exp2)
                {
                    // logger.Error(exp2);
                }

                MessageDialog.Show(exp.Message, "Error finding requested resource");

                if (mainApp.AppIsShuttingDown == false)
                    mainApp.Shutdown();
            }
        }

        void ConstructMainWindowSession()
        {
            var mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;

            var win = Application.Current.MainWindow;
            var settings = application.SettingsManager;

            win.DataContext = application;

            // Establish command binding to accept user input via commanding framework
            application.InitCommandBinding(win as ILayoutableWindow);

            //var mainWindowPos = (settings.SessionData as Profile).MainWindowPosSz;
            //win.Left = mainWindowPos.X;
            //win.Top = mainWindowPos.Y;
            //win.Width = mainWindowPos.Width;
            //win.Height = mainWindowPos.Height;
            //win.WindowState = mainWindowPos.IsMaximized == true ? WindowState.Maximized : WindowState.Normal;

            // Initialize Window State in viewmodel to show resize grip when window is not maximized
            if (win.WindowState == WindowState.Maximized)
                application.IsNotMaximized = false;
            else
                application.IsNotMaximized = true;


            var mainApp = Application.Current as MainApp;
            if (mainApp.AppIsShuttingDown == false)
                mainApp.ShutdownMode = ShutdownMode.OnLastWindowClose;


            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Interpret command line parameters and process their content
        /// </summary>
        /// <param name="args"></param>
        void ProcessCmdLine(IEnumerable<string> args)
        {
            if (args != null && args.Count() > 0)
            {
                foreach (string sPath in args)
                {
                    // Command may not be bound yet so we do this via direct call
                    application.OpenSolution(sPath);

                    break;//one solution only
                }
            }
        }
    }
}
