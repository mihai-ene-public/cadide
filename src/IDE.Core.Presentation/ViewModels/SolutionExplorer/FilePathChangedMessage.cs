namespace IDE.Core.ViewModels;

/// <summary>
/// A file that belongs to a project changes its path
/// </summary>
public class FilePathChangedMessage
{
    required public string OldFilePath { get; set; }
    required public string NewFilePath { get; set; }
}

