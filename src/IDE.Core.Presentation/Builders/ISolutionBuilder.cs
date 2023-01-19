using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Builders;

public interface ISolutionBuilder
{
    Task<IList<string>> BuildSolution(string solutionFilePath);

    Task<IList<string>> BuildProject(string filePath);

}
