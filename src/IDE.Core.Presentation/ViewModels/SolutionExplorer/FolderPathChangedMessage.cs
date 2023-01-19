namespace IDE.Core.ViewModels;

/// <summary>
/// A folder that belongs to a project changes its path
/// </summary>
public class FolderPathChangedMessage
{
    required public string OldFolderPath { get; set; }
    required public string NewFolderPath { get; set; }
}

