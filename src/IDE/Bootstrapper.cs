using IDE.App.Views.Shell;
using IDE.Core.Interfaces;
using IDE.Core.Settings;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
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
        public Bootstrapper(
            IApplicationViewModel application,
            ISettingsManager settingsManager,
            IToolWindowRegistry toolWindowRegistry,
            IAppCoreModel appCore,
            IDocumentTypeManager documentTypeManager
            )
        {
            _application = application;
            _settingsManager = settingsManager;
            _toolWindowRegistry = toolWindowRegistry;
            _appCore = appCore;
            _documentTypeManager = documentTypeManager;
        }

        private readonly IApplicationViewModel _application;
        private readonly ISettingsManager _settingsManager;
        private readonly IToolWindowRegistry _toolWindowRegistry;
        private readonly IAppCoreModel _appCore;
        private readonly IDocumentTypeManager _documentTypeManager;

        public void Run(StartupEventArgs eventArgs)
        {
            _settingsManager.LoadOptions(AppHelpers.DirFileAppSettingsData);
            //var options = _settingsManager.SettingData;

            var envSetting = _settingsManager.GetSetting<EnvironmentGeneralSetting>();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(envSetting.LanguageSelected);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(envSetting.LanguageSelected);

            Initialize();

            CreateShell();

            // Show the startpage if application starts for the very first time
            // (This requires that command binding was succesfully done before this line)
            Core.Commands.AppCommand.ShowStartPage.Execute(null);

            if (eventArgs != null)
                ProcessCmdLine(eventArgs.Args);
        }

        void CreateShell()
        {
            ConstructMainWindowSession();

            _toolWindowRegistry.PublishTools();
        }

        void ConstructMainWindowSession()
        {
            var mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;

            var win = Application.Current.MainWindow;

            win.DataContext = _application;

            // Establish command binding to accept user input via commanding framework
            _application.InitCommandBinding(win as ILayoutableWindow);

            // Initialize Window State in viewmodel to show resize grip when window is not maximized
            _application.IsNotMaximized = win.WindowState == WindowState.Maximized;

            var mainApp = Application.Current as MainApp;
            if (mainApp.AppIsShuttingDown == false)
                mainApp.ShutdownMode = ShutdownMode.OnLastWindowClose;

            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Interpret command line parameters and process their content
        /// </summary>
        /// <param name="args"></param>
        void ProcessCmdLine(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                // Command may not be bound yet so we do this via direct call
                _application.OpenSolution(args[0]);
            }
        }

        void Initialize()
        {
            RegisterEditorModels();

            RegisterServices();

            _application.LoadConfig();

            _appCore.CreateAppDataFolder();
        }

        void RegisterEditorModels()
        {
            //there is the possibility to load all these using MEF, but for now we can use this


            var registerables = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from t in assembly.GetTypes()
                                 where t.GetInterfaces().Contains(typeof(IRegisterable))
                                       && t.GetConstructor(Type.EmptyTypes) != null
                                 select t).ToList();
            foreach (var r in registerables)
            {
                try
                {
                    var instance = (IRegisterable)Activator.CreateInstance(r);

                    instance.RegisterDocumentType(_documentTypeManager);

                    if (instance is ToolViewModel)
                    {
                        var t = instance as ToolViewModel;
                        t.IsVisible = false;
                        _toolWindowRegistry.RegisterTool(t);
                    }

                }
                catch //(Exception ex)
                {
                    //log these in Output window or with log4net
                }

            }
        }


        void RegisterServices()
        {
            var registerables = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from t in assembly.GetTypes()
                                 where t.GetInterfaces().Contains(typeof(IService))
                                       && t.GetConstructor(Type.EmptyTypes) != null
                                 select t).ToList();
            foreach (var r in registerables)
            {
                try
                {
                    //it is enough to create the service; it will register by itself
                    var instance = (IService)Activator.CreateInstance(r);
                }
                catch //(Exception ex)
                {
                    //log these in Output window or with log4net
                }

            }
        }
    }
}
