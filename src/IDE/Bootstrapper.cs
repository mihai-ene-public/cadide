using IDE.App.Views.Shell;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Infrastructure;
using IDE.Core.Settings;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
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
            IDocumentTypeManager documentTypeManager,
            IServiceProviderHelper serviceProviderHelper
            )
        {
            _application = application;
            _settingsManager = settingsManager;
            _toolWindowRegistry = toolWindowRegistry;
            _appCore = appCore;
            _documentTypeManager = documentTypeManager;
            _serviceProviderHelper = serviceProviderHelper;
        }

        private readonly IApplicationViewModel _application;
        private readonly ISettingsManager _settingsManager;
        private readonly IToolWindowRegistry _toolWindowRegistry;
        private readonly IAppCoreModel _appCore;
        private readonly IDocumentTypeManager _documentTypeManager;
        private readonly IServiceProviderHelper _serviceProviderHelper;

        public void Run(StartupEventArgs eventArgs)
        {
            _settingsManager.LoadOptions(AppHelpers.DirFileAppSettingsData);

            var envSetting = _settingsManager.GetSetting<EnvironmentGeneralSetting>();

            Thread.CurrentThread.CurrentCulture = new CultureInfo(envSetting.LanguageSelected);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(envSetting.LanguageSelected);

            Initialize();

            CreateShell();

            _application.ShowStartPage();
        }

        void CreateShell()
        {
            ConstructMainWindowSession();
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

        void Initialize()
        {
            RegisterToolWindows();
            RegisterEditorModels();

            _application.LoadConfig();

            _appCore.CreateAppDataFolder();
        }

        void RegisterToolWindows()
        {
            var toolWindows = _serviceProviderHelper.GetServices<IToolWindow>();
            foreach(var tool in toolWindows)
            {
                _toolWindowRegistry.RegisterTool(tool);
            }
        }

        void RegisterEditorModels()
        {
            _documentTypeManager.RegisterDocumentType("Symbol Editor", "Symbol files", "Symbol file", "symbol", typeof(ISymbolDesignerViewModel));
            _documentTypeManager.RegisterDocumentType("Model Editor", "Model files", "Model file", "model", typeof(IMeshDesigner));
            _documentTypeManager.RegisterDocumentType("Component Editor", "Component files", "Component file", "component", typeof(IComponentDesigner));
            _documentTypeManager.RegisterDocumentType("Footprint Editor", "Footprint files", "Footprint file", "footprint", typeof(IFootprintDesigner));
            _documentTypeManager.RegisterDocumentType("Schematic Editor", "Schematic files", "Schematic file", "schematic", typeof(ISchematicDesigner));
            _documentTypeManager.RegisterDocumentType("Board Editor", "Board files", "Board file", "board", typeof(IBoardDesigner));
            _documentTypeManager.RegisterDocumentType("Solution", "Solution files", "Solution file", "solution", typeof(ISolutionExplorerToolWindow));
        }


    }
}
