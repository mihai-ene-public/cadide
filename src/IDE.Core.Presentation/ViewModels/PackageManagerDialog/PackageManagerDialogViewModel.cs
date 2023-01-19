using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Eagle;
using IDE.Core;
using IDE.Core.Presentation.Packaging;

namespace IDE.Documents.Views;
public class PackageManagerDialogViewModel : DialogViewModel
{
    private readonly IPackageManager _packageManager;

    public PackageManagerDialogViewModel(IPackageManager packageManager)
    {
        _packageManager = packageManager;

        PropertyChanged += DialogViewModel_PropertyChanged;
    }

    string _projectPath = null;

    public string WindowTitle
    {
        get
        {
            return "Package Manager";
        }
    }

    private IList<PackageInfoModel> fullSourcePackages;
    private IList<PackageInfoModel> _packages = new List<PackageInfoModel>();
    public IList<PackageInfoModel> Packages
    {
        get { return _packages; }
        set
        {
            _packages = new List<PackageInfoModel>(value);//we want a copy
            OnPropertyChanged(nameof(Packages));
        }
    }

    private PackageInfoModel selectedItem;
    public PackageInfoModel SelectedItem
    {
        get { return selectedItem; }
        set
        {
            selectedItem = value;
            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    private string searchFilter;

    public string SearchFilter
    {
        get { return searchFilter; }
        set
        {
            searchFilter = value;
            OnPropertyChanged(nameof(SearchFilter));
        }
    }

    private ICommand clearSearchFilterCommand;

    public ICommand ClearSearchFilterCommand
    {
        get
        {
            if (clearSearchFilterCommand == null)
                clearSearchFilterCommand = CreateCommand(p => { SearchFilter = null; });

            return clearSearchFilterCommand;
        }
    }

    private ICommand installCommand;

    public ICommand InstallCommand
    {
        get
        {
            if (installCommand == null)
                installCommand = CreateCommand(async p => { await InstallPackage(); });

            return installCommand;
        }
    }

    private async Task InstallPackage()
    {
        var selected = selectedItem;

        var packRef = new PackageInfoRef
        {
            PackageId = selected.PackageId,
            PackageSource = selected.PackageSource,
            PackageVersion = selected.SelectedVersion
        };
        await _packageManager.ProjectInstallPackage(_projectPath, packRef);

        await _packageManager.RestorePackage(packRef);

        selected.CurrentInstalledVersion = packRef.PackageVersion;
    }

    private ICommand uninstallCommand;

    public ICommand UninstallCommand
    {
        get
        {
            if (uninstallCommand == null)
                uninstallCommand = CreateCommand(async p => { await UninstallPackage(); });

            return uninstallCommand;
        }
    }

    private async Task UninstallPackage()
    {
        var selected = selectedItem;

        var packRef = new PackageInfoRef
        {
            PackageId = selected.PackageId,
            PackageSource = selected.PackageSource,
            PackageVersion = selected.SelectedVersion
        };
        await _packageManager.ProjectUninstallPackage(_projectPath, packRef);

        selected.CurrentInstalledVersion = null;
    }

    private async Task UpdatePackage()
    {
        var selected = selectedItem;

        var packRef = new PackageInfoRef
        {
            PackageId = selected.PackageId,
            PackageSource = selected.PackageSource,
            PackageVersion = selected.SelectedVersion
        };
        await _packageManager.ProjectUpdatePackage(_projectPath, packRef);

        await _packageManager.RestorePackage(packRef);

        selected.CurrentInstalledVersion = packRef.PackageVersion;
    }

    private ICommand updateCommand;

    public ICommand UpdateCommand
    {
        get
        {
            if (updateCommand == null)
                updateCommand = CreateCommand(async p => { await UpdatePackage(); });

            return updateCommand;
        }
    }

    private void DialogViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SearchFilter))
        {
            ApplyFilter();
        }
    }

    private void ApplyFilter()
    {
        var theNewSource = fullSourcePackages;

        if (!string.IsNullOrEmpty(searchFilter))
        {
            theNewSource = new List<PackageInfoModel>();

            theNewSource.AddRange(fullSourcePackages.Where(p => p.PackageId != null && p.PackageId.Contains(searchFilter, StringComparison.OrdinalIgnoreCase)));
        }

        ApplyFromSource(theNewSource);
    }

    private void ApplyFromSource(IList<PackageInfoModel> source)
    {
        Packages = source;
    }

    protected override async Task LoadData(object args)
    {
        _projectPath = (string)args;
        if (string.IsNullOrEmpty(_projectPath))
        {
            throw new ArgumentException(nameof(args));
        }

        var installedPackages = await _packageManager.GetProjectInstalledPackages(_projectPath);

        var packages = await _packageManager.GetPackages();
        //todo: installed packages in project

        var packs = from p in packages
                    group p by new { p.PackageId, p.PackageSource } into packGroup
                    orderby packGroup.Key.PackageId
                    let pack = packGroup.First()
                    select new PackageInfoModel
                    {
                        PackageId = packGroup.Key.PackageId,
                        PackageSource = pack.PackageSource,
                        ProjectUrl = pack.ProjectUrl,
                        Icon = pack.Icon,
                        Description = pack.Description,
                        Authors = pack.Authors,
                        Tags = pack.Tags,
                        Versions = packGroup.Select(g => g.Version).OrderDescending().ToList()
                    };

        fullSourcePackages = packs.ToList();

        foreach (var installedPackage in installedPackages)
        {
            var lookupPack = fullSourcePackages.FirstOrDefault(p => p.PackageId == installedPackage.PackageId && p.PackageSource == installedPackage.PackageSource);

            if (lookupPack != null)
            {
                lookupPack.SelectedVersion = installedPackage.PackageVersion;
                lookupPack.CurrentInstalledVersion = installedPackage.PackageVersion;
            }
        }

        ApplyFromSource(fullSourcePackages);
    }
}
