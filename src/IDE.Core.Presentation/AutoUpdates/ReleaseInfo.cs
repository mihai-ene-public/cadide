namespace IDE.Core.Presentation.AutoUpdates;

public class ReleaseInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string ReleaseAssetDownloadUrl { get; set; }

    public string AssetFileName { get; set; }

    public int DownloadSize { get; set; }
}
