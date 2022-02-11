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
using IDE.Core.Documents;
using IDE.Core.Compilation;

namespace IDE
{
    internal static class Startup
    {

        private static IServiceProvider serviceProvider;

        private static void ConfigureContainer()
        {
            var services = new ServiceCollection();

            services.AddSingleton<Bootstrapper>();
            services.AddSingleton<IApplicationViewModel, ApplicationViewModel>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IThemesManager, ThemesManager>();
            services.AddSingleton<IToolWindowRegistry, ToolWindowRegistry>();
            services.AddSingleton<IDocumentTypeManager, DocumentTypeManager>();
            services.AddSingleton<IAppCoreModel, AppCoreModel>();

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
            services.AddSingleton<IPrimitiveToCanvasItemMapper, PrimitiveToCanvasItemMapper>();
            services.AddSingleton<ISolutionExplorerNodeMapper, SolutionExplorerNodeMapper>();
            services.AddSingleton<ISettingsDataToModelMapper, SettingsDataToModelMapper>();
            services.AddSingleton<IBoardRulesDataToModelMapper, BoardRulesDataToModelMapper>();
            services.AddSingleton<ISchematicRulesToModelMapper, SchematicRulesDataToModelMapper>();
            services.AddSingleton<IFileExtensionToSolutionExplorerNodeMapper, FileExtensionToSolutionExplorerNodeMapper>();
            services.AddSingleton<ISchematicRulesToModelMapper, SchematicRulesDataToModelMapper>();
            services.AddSingleton<IDialogModelToWindowMapper, DialogModelToWindowMapper>();
            
            services.AddSingleton<IActiveCompiler, ActiveCompiler>();

            serviceProvider = services.BuildServiceProvider();
        }

        public static void Run(StartupEventArgs _eventArgs)
        {
            ConfigureContainer();
            Core.ServiceProvider.RegisterResolver((t) => serviceProvider.GetService(t));

            var app = serviceProvider.GetService<Bootstrapper>();
            app.Run(_eventArgs);
        }
    }
}
