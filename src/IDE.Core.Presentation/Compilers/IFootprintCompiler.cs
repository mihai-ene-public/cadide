using System.Threading.Tasks;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public interface IFootprintCompiler
{
    Task<CompilerResult> Compile(IFootprintDesigner footprint);
}
