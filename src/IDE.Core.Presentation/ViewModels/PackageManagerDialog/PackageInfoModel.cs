using IDE.Core;

namespace IDE.Documents.Views;

public class PackageInfoModel : BaseViewModel
{
    public string PackageId { get; set; }
    public string PackageSource { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public string ProjectUrl { get; set; }
    public string Authors { get; set; }
    public string Tags { get; set; }

    public IList<string> Versions { get; set; } = new List<string>();

    private string currentInstalledVersion;
    public string CurrentInstalledVersion
    {
        get { return currentInstalledVersion; }
        set
        {
            currentInstalledVersion = value;
            OnPropertyChanged(nameof(CurrentInstalledVersion));

            OnPropertyChanged(nameof(CanInstall));
            OnPropertyChanged(nameof(CanUpdate));
            OnPropertyChanged(nameof(CanUninstall));
        }
    }

    private string selectedVersion;
    public string SelectedVersion
    {
        get { return selectedVersion; }
        set
        {
            selectedVersion = value;
            OnPropertyChanged(nameof(SelectedVersion));

            OnPropertyChanged(nameof(CanInstall));
            OnPropertyChanged(nameof(CanUpdate));
            OnPropertyChanged(nameof(CanUninstall));
        }
    }

    public bool CanInstall => string.IsNullOrEmpty(CurrentInstalledVersion);
    public bool CanUpdate => CanUninstall
                          && Versions?.Count > 0 
                          && SelectedVersion != CurrentInstalledVersion;
    public bool CanUninstall => !string.IsNullOrEmpty(CurrentInstalledVersion);
}
