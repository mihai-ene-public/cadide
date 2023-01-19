namespace IDE.Core.Data.Packages;

public class PackageMetadataRepository
{
    /// <summary>
    /// Ex: git
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Ex: https://github.com/user/projectName
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Commit id: deadbeefxxx
    /// </summary>
    public string Commit { get; set; }
}
