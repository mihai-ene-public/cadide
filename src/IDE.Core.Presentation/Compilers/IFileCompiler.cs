using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface IFileCompiler
{
    Task<CompilerResult> Compile(IFileBaseViewModel file);
}

public interface ISolutionCompiler
{
    Task CompileSolution(ISolutionRootNodeModel solution);

    Task BuildSolution(ISolutionRootNodeModel solution);

    Task CompileProject(ISolutionProjectNodeModel project);

    Task BuildProject(ISolutionProjectNodeModel project);
}
