using IDE.Core.Interfaces;
using IDE.Core.Settings;

namespace IDE.Core.Presentation.Packaging;

/// <summary>
/// package sources are coming from settings (for IDE)
/// </summary>
public class PackageSourceSettingsRepository : IPackageSourceRepository
{
    private readonly ISettingsManager _settingsManager;

    public PackageSourceSettingsRepository(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public IList<PackageSource> GetPackageSources()
    {
        var packageSetting = _settingsManager.GetSetting<PackageManagerSettings>();

        if (packageSetting != null)
        {
            return packageSetting.PackageSources.Select(s =>
                new PackageSource
                {
                    Name = s.Name,
                    IsEnabled = s.IsEnabled,
                    Source = s.Source
                }
                ).ToList();
        }

        return new List<PackageSource>();
    }
}
