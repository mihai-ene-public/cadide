using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface ISymbolCompiler
{
    Task<CompilerResult> Compile(ISymbolDesignerViewModel symbol);
}
