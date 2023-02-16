using System.Windows.Input;
using IDE.Core;
using IDE.Core.Presentation.AutoUpdates;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.Documents.Views;

namespace IDE.Dialogs.CheckUpdatesDialog;
public class CheckUpdatesDialogViewModel : DialogViewModel
{
    public CheckUpdatesDialogViewModel(IUpdatesManager updatesManager)
    {
        _updatesManager = updatesManager;
    }

    private readonly IUpdatesManager _updatesManager;
    private ReleaseInfo _releaseInfo;

    public string WindowTitle
    {
        get
        {
            return "Check Updates";
        }
    }

    public bool IsDownloadStarted { get; set; }

    public object UpdatesContent { get; set; }

    public string CurrentVersion => AppHelpers.ApplicationVersion;

    ICommand updateCommand;

    public ICommand UpdateCommand
    {
        get
        {
            if (updateCommand == null)
                updateCommand = CreateCommand(async p => { await UpdateExecute(); });

            return updateCommand;
        }
    }

    public bool UpdateCommandVisible => UpdatesContent is NewUpdateInfoContent;

    private async Task UpdateExecute()
    {
        if (MessageDialog.Show(@"It is highly recommended to save all progress in all open instances of this software.
All instances will be automatically closed to perform the update.

Do you want to continue?", "Confirm update", Core.Interfaces.XMessageBoxButton.YesNo) == Core.Interfaces.XMessageBoxResult.Yes)
        {
            IsDownloadStarted = true;
            OnPropertyChanged(nameof(IsDownloadStarted));

            await _updatesManager.StartUpdate();
        }
    }

    protected override async Task LoadData(object args)
    {
        IsDownloadStarted = false;
        OnPropertyChanged(nameof(IsDownloadStarted));

        _releaseInfo = await _updatesManager.CheckUpdates();

        if (_releaseInfo != null)
        {
            UpdatesContent = new NewUpdateInfoContent
            {
                UpdateVersion = _releaseInfo.Version,
                DownloadSize = ((double)_releaseInfo.DownloadSize / 1024 / 1024).ToString("0.00 MB")
            };
        }
        else
        {
            UpdatesContent = new NoUpdatesContent();
        }

        OnPropertyChanged(nameof(UpdatesContent));
        OnPropertyChanged(nameof(UpdateCommandVisible));
    }
}
