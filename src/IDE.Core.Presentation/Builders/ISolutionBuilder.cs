using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Builders;

public interface ISolutionBuilder
{
    Task BuildSolution(ISolutionRootNodeModel solution);

    Task BuildSolution(string solutionFilePath);

    Task BuildProject(ISolutionProjectNodeModel project);
    Task BuildProject(string filePath);

}
