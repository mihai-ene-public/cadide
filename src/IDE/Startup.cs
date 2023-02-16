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
using IDE.Core.Errors;
using IDE.Core.Presentation.Infrastructure;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Presentation.Builders;
using IDE.Core.Presentation.Solution;
using IDE.Core.Presentation.ObjectFinding;
using Microsoft.Extensions.Caching.Memory;
using IDE.Core.Interfaces.Compilers;
using IDE.Core.Presentation.Placement;
using IDE.Core.Common.FileSystem;
using IDE.Core.Presentation.Packaging;
using IDE.Dialogs.CheckUpdatesDialog;
using IDE.Core.Presentation.AutoUpdates;

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
            services.AddSingleton<IGeometryOutlineHelper, GeometryOutlineHelper>();

            services.AddTransient<IPolygonGeometryOutlinePourProcessor, PolygonGeometryOutlinePourProcessor>();
            services.AddTransient<IPlaneGeometryOutlinePourProcessor, PlaneGeometryOutlinePourProcessor>();

            services.AddSingleton<IMeshHelper, MeshHelper>();
            services.AddTransient<IModelImporter, GenericModelImporter>();

            services.AddTransient<IDispatcherHelper, DispatcherHelper>();
            services.AddTransient<IClipboardAdapter, ClipboardAdapter>();

            services.AddTransient<IBitmapImageHelper, BitmapImageHelper>();

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

            services.AddTransient<IMemoryCache, MemoryCache>(f => new MemoryCache(new MemoryCacheOptions()));
            services.AddSingleton<ISolutionRepository, SolutionRepository>();

            services.AddSingleton(typeof(IObjectRepository<>), typeof(ObjectRepository<>));
            services.AddSingleton<IObjectRepository, ObjectRepository>();

            services.AddSingleton<ILibraryRepository, LibraryRepository>();
            
            services.AddSingleton(typeof(IObjectFinder<>), typeof(ObjectFinder<>));
            services.AddSingleton<IObjectFinder, ObjectFinder>();

            services.AddSingleton<IFileSystemProvider, FileSystemProvider>();

            services.AddTransient<PackageManagerDialogViewModel>();
            services.AddSingleton<IPackageManager, PackageManager>();
            services.AddSingleton<IPackageSourceRepository, PackageSourceSettingsRepository>();
            services.AddSingleton<IPackageRepositoryFactory, PackageRepositoryFactory>();
            services.AddSingleton<IGithubPackageRepository, GithubPackageRepository>();
            services.AddSingleton<IFolderPackageRepository, FolderPackageRepository>();

            services.AddTransient<CheckUpdatesDialogViewModel>();
            services.AddSingleton<IUpdatesManager, UpdatesManager>();
            services.AddSingleton<IReleasesManager, GithubReleasesManager>();


            services.AddSingleton<IServiceCollection>(services);
            services.AddSingleton<IServiceProviderHelper, ServiceProviderHelper>();

            AddDocumentEditors(services);
            AddToolWindows(services);
            AddCompilers(services);
            AddBuilders(services);
            AddPlacementTools(services);


            serviceProvider = services.BuildServiceProvider();
        }

        private static void AddDocumentEditors(IServiceCollection services)
        {
            services.AddTransient<ISimpleTextDocument, SimpleTextDocumentViewModel>();
            services.AddTransient<ISolutionProjectPropertiesDocument, SolutionProjectPropertiesViewModel>();
            services.AddTransient<IStartPage, StartPageViewModel>();
            services.AddTransient<IMeshDesigner, MeshDesignerViewModel>();
            services.AddTransient<ISymbolDesignerViewModel, SymbolDesignerViewModel>();
            services.AddTransient<IComponentDesigner, ComponentDesignerFileViewModel>();
            services.AddTransient<IFootprintDesigner, FootprintDesignerFileViewModel>();
            services.AddTransient<ISchematicDesigner, SchematicDesignerViewModel>();
            services.AddTransient<IBoardDesigner, BoardDesignerFileViewModel>();
        }

        private static void AddToolWindows(IServiceCollection services)
        {
            services.AddSingleton<IPropertiesToolWindow, PropertiesToolWindowViewModel>();
            services.AddSingleton<IPreview3DToolWindow, Preview3DWindowViewModel>();
            services.AddSingleton<ISchematicSheetsToolWindow, SchematicSheetsViewModel>();
            services.AddSingleton<ISelectionFilterToolWindow, SelectionFilterToolViewModel>();
            services.AddSingleton<ILayersToolWindow, LayersToolWindowViewModel>();
            services.AddSingleton<IDocumentOverviewToolWindow, DocumentOverviewViewModel>();
            services.AddSingleton<IErrorsToolWindow, ErrorsToolWindowViewModel>();
            services.AddSingleton<IOutputToolWindow, OutputToolWindow>();
            services.AddSingleton<ISolutionExplorerToolWindow, SolutionExplorerViewModel>();
        }

        private static void AddCompilers(IServiceCollection services)
        {
            services.AddSingleton<IActiveCompiler, ActiveCompiler>();
            services.AddSingleton<IFileCompiler, FileCompiler>();

            services.AddTransient<ISolutionCompiler, SolutionCompiler>();
            services.AddTransient<ISolutionBuilder, SolutionBuilder>();
            services.AddTransient<IFileCompiler, FileCompiler>();

            services.AddTransient<ISchematicRulesCompiler, SchematicRulesCompiler>();
            services.AddTransient<IBoardRulesCompiler, BoardRulesCompiler>();

            services.AddTransient<IBoardCompiler, BoardCompiler>();
            services.AddTransient<ISchematicCompiler, SchematicCompiler>();
            services.AddTransient<IComponentCompiler, ComponentCompiler>();
            services.AddTransient<ISymbolCompiler, SymbolCompiler>();
            services.AddTransient<IFootprintCompiler, FootprintCompiler>();
        }

        private static void AddBuilders(IServiceCollection services)
        {
            services.AddTransient<IBoardBuilder, BoardBuilder>();
            services.AddTransient<ISchematicBuilder, SchematicBuilder>();
            services.AddTransient<ISolutionPackageBuilder, SolutionPackageBuilder>();
        }

        private static void AddPlacementTools(IServiceCollection services)
        {
            services.AddSingleton<IPlacementToolFactory, PlacementToolFactory>();

            services.AddTransient<ISphereMeshItemPlacementTool, SphereMeshItemPlacementTool>();
            services.AddTransient<IBoxMeshItemPlacementTool, BoxMeshItemPlacementTool>();
            services.AddTransient<ITextMeshItemPlacementTool, TextMeshItemPlacementTool>();
            services.AddTransient<IConeMeshItemPlacementTool, ConeMeshItemPlacementTool>();
            services.AddTransient<ICylinderMeshItemPlacementTool, CylinderMeshItemPlacementTool>();
            services.AddTransient<IEllipsoidMeshItemPlacementTool, EllipsoidMeshItemPlacementTool>();
            services.AddTransient<IExtrudedPolyMeshItemPlacementTool, ExtrudedPolyMeshItemPlacementTool>();
            services.AddTransient<IVolatileGroup3DPlacementTool, VolatileGroup3DPlacementTool>();
            
            services.AddTransient<IVolatileGroupPlacementTool, VolatileGroupPlacementTool>();
            services.AddTransient<ILinePlacementTool, LinePlacementTool>();
            services.AddTransient<IArcPlacementTool, ArcPlacementTool>();
            services.AddTransient<ITextPlacementTool, TextPlacementTool>();
            services.AddTransient<IRectanglePlacementTool, RectanglePlacementTool>();
            services.AddTransient<IPolygonPlacementTool, PolygonPlacementTool>();
            services.AddTransient<ICirclePlacementTool, CirclePlacementTool>();
            services.AddTransient<IEllipsePlacementTool, EllipsePlacementTool>();
            services.AddTransient<IImagePlacementTool, ImagePlacementTool>();
            services.AddTransient<IPinPlacementTool, PinPlacementTool>();
            services.AddTransient<IHolePlacementTool, HolePlacementTool>();
            services.AddTransient<IPadPlacementTool, PadPlacementTool>();
            services.AddTransient<INetWirePlacementTool, NetWirePlacementTool>();
            services.AddTransient<IBusWirePlacementTool, BusWirePlacementTool>();
            services.AddTransient<IJunctionPlacementTool, JunctionPlacementTool>();
            services.AddTransient<INetLabelPlacementTool, NetLabelPlacementTool>();
            services.AddTransient<IBusLabelPlacementTool, BusLabelPlacementTool>();
            services.AddTransient<IPartPlacementTool, PartPlacementTool>();
            services.AddTransient<IViaPlacementTool, ViaPlacementTool>();


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
