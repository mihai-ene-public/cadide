using IDE.Core;
using IDE.Core.Collision;
using IDE.Core.Commands;
using IDE.Core.Common.FileSystem;
using IDE.Core.Common.Geometries;
using IDE.Core.Designers;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Compilers;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Presentation.Builders;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Presentation.Infrastructure;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace IDE.Cli;

internal static class Startup
{
    public static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDocumentTypeManager, DocumentTypeManager>();
        services.AddSingleton<IAppCoreModel, AppCoreModel>();

        services.AddTransient<IDebounceDispatcher, DebounceDispatcher>();
        services.AddSingleton<IGeometryOutlineHelper, GeometryOutlineHelper>();

        services.AddTransient<IPolygonGeometryOutlinePourProcessor, PolygonGeometryOutlinePourProcessor>();
        services.AddTransient<IPlaneGeometryOutlinePourProcessor, PlaneGeometryOutlinePourProcessor>();

        services.AddSingleton<IMeshHelper, MeshHelper>();

        services.AddTransient<IDispatcherHelper, NullDispatcherHelper>();
        services.AddTransient<IBitmapImageHelper, BitmapImageHelper>();

        services.AddTransient<ICommandFactory, NullCommandFactory>();

        services.AddSingleton<IDirtyMarkerTypePropertiesMapper, DirtyMarkerTypePropertiesMapper>();
        services.AddSingleton<IPrimitiveToCanvasItemMapper, PrimitiveToCanvasItemMapper>();
        services.AddSingleton<ISolutionExplorerNodeMapper, SolutionExplorerNodeMapper>();
        services.AddSingleton<ISettingsDataToModelMapper, SettingsDataToModelMapper>();
        services.AddSingleton<IBoardRulesDataToModelMapper, BoardRulesDataToModelMapper>();
        services.AddSingleton<ISchematicRulesToModelMapper, SchematicRulesDataToModelMapper>();
        services.AddSingleton<IFileExtensionToSolutionExplorerNodeMapper, FileExtensionToSolutionExplorerNodeMapper>();
        services.AddSingleton<ISchematicRulesToModelMapper, SchematicRulesDataToModelMapper>();

        services.AddTransient<IMemoryCache, MemoryCache>(f => new MemoryCache(new MemoryCacheOptions()));
        services.AddSingleton<ISolutionRepository, SolutionRepository>();

        services.AddSingleton(typeof(IObjectRepository<>), typeof(ObjectRepository<>));
        services.AddSingleton<IObjectRepository, ObjectRepository>();

        services.AddSingleton<ILibraryRepository, LibraryRepository>();

        services.AddSingleton(typeof(IObjectFinder<>), typeof(ObjectFinder<>));
        services.AddSingleton<IObjectFinder, ObjectFinder>();

        services.AddSingleton<IFileSystemProvider, FileSystemProvider>();

        services.AddSingleton<IServiceCollection>(services);
        services.AddSingleton<IServiceProviderHelper, ServiceProviderHelper>();

        AddDocumentEditors(services);
        AddCompilers(services);
        AddBuilders(services);


        var serviceProvider = services.BuildServiceProvider();
        Core.ServiceProvider.RegisterResolver((t) => serviceProvider.GetService(t));

        RegisterEditorModels(serviceProvider);

        return serviceProvider;
    }

    private static void RegisterEditorModels(IServiceProvider serviceProvider)
    {
        var documentTypeManager = serviceProvider.GetService<IDocumentTypeManager>();

        documentTypeManager.RegisterDocumentType("Text Editor", "Text files", "Text file", "txt", typeof(ISimpleTextDocument), null);
        documentTypeManager.RegisterDocumentType("Symbol Editor", "Symbol files", "Symbol file", "symbol", typeof(ISymbolDesignerViewModel), typeof(Symbol));
        documentTypeManager.RegisterDocumentType("Model Editor", "Model files", "Model file", "model", typeof(IMeshDesigner),typeof(ModelDocument));
        documentTypeManager.RegisterDocumentType("Component Editor", "Component files", "Component file", "component", typeof(IComponentDesigner), typeof(ComponentDocument));
        documentTypeManager.RegisterDocumentType("Footprint Editor", "Footprint files", "Footprint file", "footprint", typeof(IFootprintDesigner), typeof(Footprint));
        documentTypeManager.RegisterDocumentType("Schematic Editor", "Schematic files", "Schematic file", "schematic", typeof(ISchematicDesigner), typeof(SchematicDocument));
        documentTypeManager.RegisterDocumentType("Board Editor", "Board files", "Board file", "board", typeof(IBoardDesigner), typeof(BoardDocument));
        documentTypeManager.RegisterDocumentType("Solution", "Solution files", "Solution file", "solution", typeof(ISolutionExplorerToolWindow), null);
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

}
