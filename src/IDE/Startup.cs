using IDE.Core;
using IDE.Core.Collision;
using IDE.Core.Commands;
using IDE.Core.Common.Geometries;
using IDE.Core.Designers;
using IDE.Core.Dialogs;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.MRU;
using IDE.Core.Resources;
using IDE.Core.Settings;
using IDE.Core.Themes;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IDE
{
    class Startup
    {

        /* usage: in app startup:
         *      var start = new Startup();
         *      start.Run();
         * 
         */

        public Startup()
        {
            ConfigureContainer();
        }

        private IServiceProvider serviceProvider;

        private void ConfigureContainer()
        {
            var services = new ServiceCollection();

            services.AddSingleton<Bootstrapper>();
            services.AddSingleton<IApplicationViewModel, ApplicationViewModel>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IThemesManager, ThemesManager>();
            services.AddSingleton<IRecentFilesViewModel, RecentFilesModel>();

            services.AddTransient<IDebounceDispatcher, DebounceDispatcher>();
            services.AddSingleton<IGeometryHelper, GeometryHelper>();
            services.AddSingleton<IGeometryOutlineHelper, GeometryOutlineHelper>();

            services.AddTransient<IPolygonGeometryPourProcessor, PolygonGeometryPourProcessor>();
            services.AddTransient<IPlaneGeometryPourProcessor, PlaneGeometryPourProcessor>();

            services.AddTransient<IPolygonGeometryOutlinePourProcessor, PolygonGeometryOutlinePourProcessor>();

            services.AddSingleton<IMeshHelper,MeshHelper>();
            services.AddTransient<IModelImporter, GenericModelImporter>();


            services.AddTransient<IDispatcherHelper,DispatcherHelper>();
            services.AddTransient<IClipboardAdapter,ClipboardAdapter>();

            services.AddTransient<IBitmapImageHelper,BitmapImageHelper>();

            services.AddTransient<ICommandFactory, CommandFactory>();
            services.AddTransient<IMessageBoxDialogHelper, MessageBoxDialogHelper>();
            services.AddTransient<IItemPickerDialog, ItemPickerDialog>();

            services.AddTransient<IFolderSelectDialog, FolderSelectDialogAdapter>();
            services.AddTransient<IOpenFileDialog, OpenFileDialogAdapter>();
            services.AddTransient<ISaveFileDialog, SaveFileDialogAdapter>();

            services.AddSingleton<IResourceLocator, ResourceLocator>();

            services.AddSingleton<IDirtyMarkerTypePropertiesMapper, DirtyMarkerTypePropertiesMapper>();

            serviceProvider = services.BuildServiceProvider();
        }

        public void Run(StartupEventArgs _eventArgs)
        {
            Core.ServiceProvider.RegisterResolver((t) => serviceProvider.GetService(t));

            var app = serviceProvider.GetService<Bootstrapper>();
            app.Run(_eventArgs);
        }
    }
}
