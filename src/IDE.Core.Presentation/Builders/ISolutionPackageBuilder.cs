namespace IDE.Core.Presentation.Builders;

public interface ISolutionPackageBuilder
{
    Task BuildSolution(string solutionFilePath);

    Task BuildProject(string filePath);
}
