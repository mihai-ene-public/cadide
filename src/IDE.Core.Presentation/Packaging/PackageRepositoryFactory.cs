using IDE.Core.Common.Extensions;

namespace IDE.Core.Presentation.Packaging;

public class PackageRepositoryFactory : IPackageRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PackageRepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPackageRepository Create(string packageSource)
    {
        if (IsIndexFile(packageSource))
        {
            return _serviceProvider.GetService<IGithubPackageRepository>();
        }
        else if (IsFolder(packageSource))
        {
            return _serviceProvider.GetService<IFolderPackageRepository>();
        }

        return null;
    }

    private bool IsFolder(string path)
    {
        var uri = new Uri(path);
        var lastSegment = uri.Segments.Last();

        return uri.Scheme == Uri.UriSchemeFile && !lastSegment.Equals("index.json", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsIndexFile(string path)
    {
        var uri = new Uri(path);

        var lastSegment = uri.Segments.Last();

        return (uri.Scheme == Uri.UriSchemeHttp ||
               uri.Scheme == Uri.UriSchemeHttps) &&
               lastSegment.Equals("index.json", StringComparison.OrdinalIgnoreCase);

    }
}
