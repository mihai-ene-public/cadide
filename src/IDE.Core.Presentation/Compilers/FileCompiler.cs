using System;
using System.Threading.Tasks;
using IDE.Core.Common.Extensions;
using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public class FileCompiler : IFileCompiler
{
    private readonly IServiceProvider _serviceProvider;

    public FileCompiler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<CompilerResult> Compile(IFileBaseViewModel file)
    {
        //mapping
        switch (file)
        {

            case IBoardDesigner board:
                {
                    var compiler = _serviceProvider.GetService<IBoardCompiler>();
                    var result = await compiler.Compile(board);
                    return result;
                }
            case ISchematicDesigner schematic:
                {
                    var compiler = _serviceProvider.GetService<ISchematicCompiler>();
                    var result = await compiler.Compile(schematic);
                    return result;
                }
            case IComponentDesigner componentDesigner:
                {
                    var compiler = _serviceProvider.GetService<IComponentCompiler>();
                    var result = await compiler.Compile(componentDesigner);
                    return result;
                }
            case IFootprintDesigner footprintDesigner:
                {
                    var compiler = _serviceProvider.GetService<IFootprintCompiler>();
                    var result = await compiler.Compile(footprintDesigner);
                    return result;
                }
            case ISymbolDesignerViewModel symbolDesigner:
                {
                    var compiler = _serviceProvider.GetService<ISymbolCompiler>();
                    var result = await compiler.Compile(symbolDesigner);
                    return result;
                }
        }

        return null;
    }
}