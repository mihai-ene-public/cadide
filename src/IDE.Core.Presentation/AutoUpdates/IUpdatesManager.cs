namespace IDE.Core.Presentation.AutoUpdates;

public interface IUpdatesManager
{
    /// <summary>
    /// returns latest release info if a newer update version exists
    /// </summary>
    Task<ReleaseInfo> CheckUpdates();

    Task StartUpdate();
}