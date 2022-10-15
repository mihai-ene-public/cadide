using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface IFileCompiler
{
    Task<CompilerResult> Compile(IFileBaseViewModel file);
}
