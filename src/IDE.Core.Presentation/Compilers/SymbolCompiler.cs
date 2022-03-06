using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public class SymbolCompiler : AbstractCompiler, ISymbolCompiler
{
    public async Task<CompilerResult> Compile(ISymbolDesignerViewModel symbol)
    {
        var project = symbol.ProjectNode;
        var canvasModel = symbol.CanvasModel;
        var errors = new List<IErrorMessage>();
        var isValid = true;

        await Task.CompletedTask;

        //multiple pins in the same location
        var gpins = canvasModel.Items.OfType<PinCanvasItem>().GroupBy(p => new { p.X, p.Y }).Where(g => g.Count() > 1);

        foreach (var g in gpins)
        {
            var msg = $"Error: There are {g.Count()} pins at the same location: ({g.Key.X}, {g.Key.Y})";
            errors.Add(BuildErrorMessage(msg, project.Name, symbol));
        }

        isValid = gpins.Count() == 0;

        return new CompilerResult
        {
            Success = isValid,
            Errors = errors
        };
    }
}