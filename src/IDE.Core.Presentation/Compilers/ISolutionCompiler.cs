using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface ISolutionCompiler
{
    Task<bool> CompileSolution(string filePath);

    Task<bool> CompileProject(string filePath);

}
