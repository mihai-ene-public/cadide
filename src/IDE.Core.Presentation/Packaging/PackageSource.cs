namespace IDE.Core.Presentation.Packaging;

public class PackageSource
{
    public bool IsEnabled { get; set; } = true;
    public string Name { get; set; }

    /// <summary>
    /// Url or a folder path
    /// </summary>
    public string Source { get; set; }
}
