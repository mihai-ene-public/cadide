using IDE.Core.Storage;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.ObjectFinding;

public interface ILibraryRepository
{
    string FindLibraryFilePath(string libraryName);

    IList<string> GetAllLibrariesFilePaths();

    IList<string> GetAllLibrariesFilePaths(string packageId, string packageVersion);
}
