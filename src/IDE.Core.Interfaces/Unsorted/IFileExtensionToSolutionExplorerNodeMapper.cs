namespace IDE.Core.Interfaces
{
    public interface IFileExtensionToSolutionExplorerNodeMapper
    {
        ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(string extension);
    }
}
