using IDE.Core.Commands;
using IDE.Core.Controls;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using IDE.Core.Presentation.Solution;
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Presentation.Messages;

namespace IDE.Core.ViewModels;

/// <summary>
/// this represents an item node displayed in tree: Solution, Project, Files, References, Folders, etc
/// <para>It also takes care of document files (load, save) and commands</para>
/// </summary>
public class SolutionExplorerNodeModel : BaseViewModel, IEditBox, ISolutionExplorerNodeModel
{

    /// <summary>
    /// Expose an event that is triggered when the viewmodel requests its view to
    /// start the editing mode for rename this item.
    /// </summary>
    public event RequestEditEventHandler RequestEdit;

    public SolutionExplorerNodeModel()
    {
        dispatcherHelper = ServiceProvider.Resolve<IDispatcherHelper>();
        _solutionRepository = ServiceProvider.Resolve<ISolutionRepository>();

        Children = new SortableObservableCollection<ISolutionExplorerNodeModel>(new ExplorerNodeComparer());
        //rename is disabled by default
        IsReadOnly = false;

        Messenger.Register<ISolutionExplorerNodeModel, FolderPathChangedMessage>(this,
              (vm, message) =>
              {
                  var oldItemPath = GetItemFullPath();

                  if (PathHelper.FolderPathContainsFile(message.OldFolderPath, oldItemPath))
                  {
                      var oldFileFolder = Path.GetDirectoryName(oldItemPath);
                      var oldFileRelativePath = Path.GetRelativePath(message.OldFolderPath, oldItemPath);
                      fileFullPath = Path.Combine(message.NewFolderPath, oldFileRelativePath);
                  }
              });
    }

    private readonly IDispatcherHelper dispatcherHelper;
    private readonly ISolutionRepository _solutionRepository;

    public string Name
    {
        get
        {
            return GetNameInternal();
        }
        set
        {
            SetNameInternal(value);
            OnPropertyChanged(nameof(Name));
        }
    }

    bool isSelected;
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    bool isEditing;
    public bool IsEditing
    {
        get
        {
            return isEditing;
        }
        set
        {
            isEditing = value;
            OnPropertyChanged(nameof(IsEditing));
        }
    }

    bool isCut;
    public bool IsCut
    {
        get
        {
            return isCut;
        }
        set
        {
            isCut = value;
            OnPropertyChanged(nameof(IsCut));
        }
    }

    bool isExpanded = true;
    public bool IsExpanded
    {
        get
        {
            return isExpanded;
        }
        set
        {
            isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        }
    }

    /// <summary>
    /// the name of the folder or file including extension. Not the full path
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// the full path to the file in this node
    /// </summary>
    protected string fileFullPath;

    /// <summary>
    /// if this instance is a file node, it represents the Project node it belongs to
    /// <para>may be NULL otherwise</para>
    /// </summary>
    public ISolutionProjectNodeModel ProjectNode
    {
        get
        {
            if (this is SolutionProjectNodeModel)
            {
                return this as SolutionProjectNodeModel;
            }
            else
            {
                ISolutionExplorerNodeModel parent = ParentNode;
                while (parent != null)
                {
                    if (parent is SolutionProjectNodeModel)
                    {
                        return parent as SolutionProjectNodeModel;
                    }

                    parent = parent.ParentNode;
                }

                return null;
            }
        }
    }

    public ISolutionRootNodeModel SolutionNode
    {
        get
        {

            if (this is SolutionRootNodeModel)
            {
                return this as SolutionRootNodeModel;
            }
            else
            {
                var parent = ParentNode;
                while (parent != null)
                {
                    if (parent is SolutionRootNodeModel)
                    {
                        return parent as SolutionRootNodeModel;
                    }

                    parent = parent.ParentNode;
                }

                return null;
            }
        }
    }


    public ISolutionExplorerNodeModel ParentNode { get; set; }

    public IList<ISolutionExplorerNodeModel> Children { get; private set; }


    bool isReadOnly;
    /// <summary>
    /// Gets/sets whether this node is readonly (can be renamed) or not.
    /// A drive can, for example, not be renamed and is therefore, readonly
    /// on this context.
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            return isReadOnly;
        }
        protected set
        {
            isReadOnly = value;
            OnPropertyChanged(nameof(IsReadOnly));
        }
    }




    ICommand renameCommand;

    public ICommand RenameCommand
    {
        get
        {

            if (renameCommand == null)
                renameCommand = CreateCommand(it =>
                {
                    var tuple = it as Tuple<string, object>;

                    if (tuple != null)
                    {
                        var newName = tuple.Item1;

                        if (string.IsNullOrEmpty(newName))
                            return;

                        Name = newName;
                    }
                }
               , canClose => !IsReadOnly);

            return renameCommand;
        }
    }

    ICommand beginRenameCommand;
    public ICommand BeginRenameCommand
    {
        get
        {
            if (beginRenameCommand == null)
            {
                beginRenameCommand = CreateCommand(it =>
                {
                    BeginRename();
                }
                , p => !IsReadOnly
                );
            }

            return beginRenameCommand;
        }
    }

    public void BeginRename()
    {
        IsEditing = true;
    }

    ICommand openContainingFolderCommand;
    public ICommand OpenContainingFolderCommand
    {
        get
        {
            if (openContainingFolderCommand == null)
                openContainingFolderCommand = CreateCommand((p) => OpenContainingFolder());

            return openContainingFolderCommand;
        }
    }

    public void DeleteItem()
    {
        try
        {
            if (this is ProjectReferenceNodeModel projectReferenceNodeModel)
            {
                var projFilePath = ProjectNode.GetItemFullPath();
                _solutionRepository.ProjectRemoveReference(projFilePath, projectReferenceNodeModel.Name);
            }
            else if (this is SolutionProjectNodeModel projectNode)
            {
                var solutionNode = SolutionNode;
                var solutionPath = solutionNode.GetItemFullPath();
                _solutionRepository.SolutionRemoveProject(solutionPath, projectNode.Name);
            }
            else
            {
                //remove the folder from disk with all the children files
                var path = GetItemFullPath();

                if (PathHelper.IsDirectory(path))
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    File.Delete(path);
                }
            }

            ParentNode.Children.Remove(this);
        }
        catch (Exception ex)
        {
            MessageDialog.Show(ex.Message);
        }
    }

    /// <summary>
    /// Opens the folder in which this document is stored in the Windows Explorer.
    /// </summary>
    private void OpenContainingFolder()
    {
        var filePath = string.Empty;
        try
        {
            filePath = GetItemFullPath();
            if (File.Exists(filePath))
            {
                // combine the arguments together it doesn't matter if there is a space after ','
                var argument = @"/select, " + filePath;

                Process.Start("explorer.exe", argument);
            }
            else
            {
                //get the directory of this item (it can be itself a folder)
                var directory = Directory.Exists(filePath) ? filePath : Directory.GetParent(filePath).FullName;

                if (!Directory.Exists(directory))
                {
                    throw new Exception($"Folder {directory} could not be found.");
                }
                else
                {
                    var argument = directory;

                    Process.Start("explorer.exe", argument);
                }
            }
        }
        catch (Exception ex)
        {
            MessageDialog.Show(ex.Message);
        }
    }


    /// <summary>
    /// when overridden it will get the name text for displaying
    /// </summary>
    /// <returns></returns>
    protected virtual string GetNameInternal()
    {
        return FileName;
    }

    /// <summary>
    /// when overridded it will set the name (like renaming a file)
    /// <para>not all names can be changed</para>
    /// </summary>
    protected void SetNameInternal(string name)
    {
        //calling this method by property changing seems a bit dangerous, because it's renaming files on disk
        //todo: we should review this approach

        var oldItemPath = GetItemFullPath();

        if (string.IsNullOrEmpty(oldItemPath))
            return;

        var oldItemFolder = Path.GetDirectoryName(oldItemPath);
        var oldExtension = Path.GetExtension(oldItemPath);
        //keep the old extension
        var fileName = Path.GetFileNameWithoutExtension(name) + oldExtension;

        var newFilePath = Path.Combine(oldItemFolder, fileName);

        var solutionRepo = ServiceProvider.Resolve<ISolutionRepository>();

        if (this is ISolutionProjectNodeModel projectNodeModel)
        {
            var solFilePath = SolutionNode.GetItemFullPath();

            solutionRepo.RenameProject(solFilePath, oldItemPath, newFilePath);
        }

        //if (Document is ProjectDocument pd)//rename project
        //{
        //    var solFolder = Path.GetDirectoryName(SolutionManager.SolutionFilePath);
        //    //var pd = (ProjectDocument)Document;
        //    pd.FilePath = Path.Combine(oldItemFolder, fileName);
        //    ( this as SolutionProjectNodeModel ).FileItem.RelativePath = DirectoryName.GetRelativePath(solFolder, pd.FilePath);
        //    FileName = fileName;
        //    //todo we should rename the project folder

        //    SolutionManager.SaveSolution();
        //}
        //else if (Document is SolutionDocument)//rename solution
        //{
        //    SolutionManager.SolutionFilePath = Path.Combine(oldItemFolder, fileName);
        //}
        //else//any other file or folder 
        //{
        //    FileName = fileName;
        //}

        FileName = fileName;
        fileFullPath = newFilePath;

        if (this is ISolutionRootNodeModel)
        {
            //notify solution changed
            Messenger.Send(new SolutionFilePathChangedMessage
            {
                NewSolutionFilePath = newFilePath
            });
        }

        var newItemPath = GetItemFullPath();

        if (oldItemPath.ToLower() == newItemPath.ToLower())
            return;

        //rename the file/folder on disk
        if (PathHelper.IsDirectory(oldItemPath))
        {
            Directory.Move(oldItemPath, newItemPath);

            //notify file changed
            Messenger.Send(new FolderPathChangedMessage
            {
                NewFolderPath = newFilePath,
                OldFolderPath = oldItemPath
            });
        }
        else
        {
            File.Move(oldItemPath, newItemPath);

            //notify file changed
            Messenger.Send(new FilePathChangedMessage
            {
                NewFilePath = newFilePath,
                OldFilePath = oldItemPath
            });
        }

       
    }

    /// <summary>
    /// intended to load the document, update/load the children
    /// </summary>
    /// <param name="filePath"></param>
    public virtual void Load(string filePath)
    {
        FileName = Path.GetFileName(filePath);
        fileFullPath = filePath;
    }

    public string GetItemFullPath()
    {
        return fileFullPath;

        //if (Document is SolutionDocument)
        //    return SolutionManager.SolutionFilePath;
        //if (Document is ProjectDocument)
        //    return ( Document as ProjectDocument ).FilePath;

        //// ProjectBaseFile f;f.

        //var parent = ParentNode;
        //var filePath = FileName;

        //if (string.IsNullOrWhiteSpace(filePath))
        //    return null;

        ////parent.FileItem.RelativePath
        //while (parent != null)
        //{
        //    if (parent.Document is ProjectDocument)
        //    {
        //        filePath = Path.Combine(Path.GetDirectoryName(( parent.Document as ProjectDocument ).FilePath),
        //                                filePath);

        //        return filePath;
        //    }
        //    filePath = Path.Combine(parent.FileName, filePath);

        //    parent = parent.ParentNode;
        //}

        //return filePath;

    }

    /// <summary>
    /// returns the actual folder if the item is a folder or the folder of the item file
    /// </summary>
    /// <returns></returns>
    public string GetItemFolderFullPath()
    {
        var filePath = GetItemFullPath();
        if (this is ProjectFolderNodeModel)
            return filePath;

        filePath = Path.GetDirectoryName(filePath);
        return filePath;
    }

    public ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(string fileExtension)
    {
        var mapper = ServiceProvider.Resolve<IFileExtensionToSolutionExplorerNodeMapper>();
        var item = mapper.CreateSolutionExplorerNodeModel(fileExtension);

        return item;
    }

    public ISolutionExplorerNodeModel CreateSolutionExplorerFolderNodeModel(string folderPath)
    {
        var f = new ProjectFolderNodeModel();
        f.Load(folderPath);
        return f;
    }

    public void AddChild(ISolutionExplorerNodeModel child)
    {
        if (dispatcherHelper == null)
        {
            Children.Add(child);
            child.ParentNode = this;
        }
        else
        {
            dispatcherHelper.RunOnDispatcher(() =>
                    {
                        Children.Add(child);
                        child.ParentNode = this;
                    });
        }

    }

    public void AddChildren(IEnumerable<ISolutionExplorerNodeModel> children)
    {
        foreach (var child in children)
        {
            Children.Add(child);
            child.ParentNode = this;
        }
    }

    public SolutionExplorerNodeModel CloneNode()
    {
        return (SolutionExplorerNodeModel)MemberwiseClone();
    }

    public override string ToString()
    {
        return Name;
    }
}

