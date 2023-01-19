using IDE.Core;
using IDE.Core.Data.Packages;

namespace IDE.Documents.Views;

public class PackagePropertiesViewModel : BaseViewModel
{


    string packageId;
    public string PackageId
    {
        get
        {
            return packageId;
        }
        set
        {
            packageId = value;
            OnPropertyChanged(nameof(PackageId));
        }
    }

    string packageVersion;
    public string PackageVersion
    {
        get
        {
            return packageVersion;
        }
        set
        {
            packageVersion = value;
            OnPropertyChanged(nameof(PackageVersion));
        }
    }

    string packageIcon;
    public string PackageIcon
    {
        get
        {
            return packageIcon;
        }
        set
        {
            packageIcon = value;
            OnPropertyChanged(nameof(PackageIcon));
        }
    }

    string packageDescription;
    public string PackageDescription
    {
        get
        {
            return packageDescription;
        }
        set
        {
            packageDescription = value;
            OnPropertyChanged(nameof(PackageDescription));
        }
    }

    string packageAuthors;
    public string PackageAuthors
    {
        get
        {
            return packageAuthors;
        }
        set
        {
            packageAuthors = value;
            OnPropertyChanged(nameof(PackageAuthors));
        }
    }

    string projectUrl;
    public string ProjectUrl
    {
        get
        {
            return projectUrl;
        }
        set
        {
            projectUrl = value;
            OnPropertyChanged(nameof(ProjectUrl));
        }
    }

    string packageTags;
    public string PackageTags
    {
        get
        {
            return packageTags;
        }
        set
        {
            packageTags = value;
            OnPropertyChanged(nameof(PackageTags));
        }
    }

    internal void LoadFrom(PackageMetadata packageMetadata)
    {
        if (packageMetadata == null)
            return;

        PackageId = packageMetadata.Id;
        PackageVersion = packageMetadata.Version;
        PackageIcon = packageMetadata.Icon;
        PackageDescription = packageMetadata.Description;
        PackageAuthors = packageMetadata.Authors;
        ProjectUrl = packageMetadata.ProjectUrl;
        PackageTags = packageMetadata.Tags;
    }

    internal PackageMetadata ToPackageMetadata()
    {
        return new PackageMetadata
        {
            Id = PackageId,
            Version = PackageVersion,
            Icon = PackageIcon,
            Description = PackageDescription,
            Authors = PackageAuthors,
            ProjectUrl = ProjectUrl,
            Tags = PackageTags
        };
    }
}
