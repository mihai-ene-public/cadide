namespace IDE.Core.Presentation.Packaging;

public interface IPackageRepositoryFactory
{
    IPackageRepository Create(string packageSource);
}
