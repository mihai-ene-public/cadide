using System.Threading.Tasks;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public interface IComponentCompiler
{
    Task<CompilerResult> Compile(IComponentDesigner component);
}
