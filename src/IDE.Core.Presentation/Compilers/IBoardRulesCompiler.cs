using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface IBoardRulesCompiler
{
    Task<CompilerResult> Compile(IBoardDesigner board);
}
