namespace IDE.Core.ViewModels
{
    using System;
    using System.Globalization;
    using System.Windows.Input;
    using System.IO;
    using System.Diagnostics;
    using System.ComponentModel;
    using Documents;
    using Utilities;
    using Commands;
    using IDE.Core.Presentation.Resources;
    using IDE.Core.Errors;
    using System.Collections.Generic;
    using IDE.Core.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;
    using IDE.Core.Types.Media;
    using IDE.Core.Presentation.Solution;
    using CommunityToolkit.Mvvm.Messaging;

    /// <summary>
    /// Base class that shares common properties, methods, and intefaces
    /// among viewmodels that represent documents
    /// </summary>
    public abstract class FileBaseViewModel : PaneViewModel, IFileBaseViewModel
    {

        protected FileBaseViewModel()
        {
            _clipBoard = ServiceProvider.Resolve<IClipboardAdapter>();
            _applicationViewModel = ServiceProvider.Resolve<IApplicationViewModel>();
            _solutionRepository = ServiceProvider.Resolve<ISolutionRepository>();

            PropertyChanged += FileBaseViewModel_PropertyChanged;

            StrongReferenceMessenger.Default.Register<IFileBaseViewModel, FilePathChangedMessage>(this,
               (vm, message) =>
               {
                   var oldItemPath = FilePath;

                   if (oldItemPath == message.OldFilePath)
                   {
                       FilePath = message.NewFilePath;
                   }
               });

            StrongReferenceMessenger.Default.Register<IFileBaseViewModel, FolderPathChangedMessage>(this,
              (vm, message) =>
              {
                  var oldItemPath = FilePath;

                  if (PathHelper.FolderPathContainsFile(message.OldFolderPath, oldItemPath))
                  {
                      var oldFileFolder = Path.GetDirectoryName(oldItemPath);
                      var oldFileRelativePath = Path.GetRelativePath(message.OldFolderPath, oldItemPath);
                      FilePath = Path.Combine(message.NewFolderPath, oldFileRelativePath);
                  }
              });
        }

        protected readonly IClipboardAdapter _clipBoard;

        protected readonly IApplicationViewModel _applicationViewModel;
        protected readonly ISolutionRepository _solutionRepository;

        async void FileBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsActive):
                    {
                        if (IsActive)
                        {
                            await Task.Run(() => RefreshFromCache());
                        }
                        break;
                    }
            }
        }

        protected virtual void RefreshFromCache()
        { }

        #region Fields

        protected object lockObject = new object();

        DocumentState documentState = DocumentState.IsLoading;

        ICommand openContainingFolderCommand = null;
        ICommand copyFullPathtoClipboard = null;

        //protected string documentTypeKey = string.Empty;

        //protected string defaultFileType = "sch";
        //protected string defaultFileName = "Schema";

        //protected string DocumentKey;
        //protected string Description;
        //protected string FileFilterName;
        //protected string DefaultFilter;

        #endregion Fields

        #region properties

        //public string DocumentTypeKey
        //{
        //    get
        //    {
        //        return documentTypeKey;
        //    }
        //}


        #region Title
        /// <summary>
        /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
        /// </summary>
        public override string Title
        {
            get
            {
                return FileName + (IsDirty ? "*" : string.Empty);
            }
        }
        #endregion

        /// <summary>
        /// Indicate whether error on load is displayed to user or not.
        /// </summary>
        protected bool CloseOnErrorWithoutMessage { get; set; }

        /// <summary>
        /// Gets the current state of the document. States may differ during
        /// initialization, loading, and other asynchron processings...
        /// </summary>
        public DocumentState State
        {
            get
            {
                lock (lockObject)
                {
                    return documentState;
                }
            }

            set
            {
                lock (lockObject)
                {
                    if (documentState != value)
                    {
                        documentState = value;

                        OnPropertyChanged(nameof(State));
                    }
                }
            }
        }

        protected string filePath;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            protected set
            {
                if (filePath != value)
                {
                    filePath = value;

                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(Title));

                }
            }
        }

        //protected void InternalGetFilePath()
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //        filePath = GetDefaultFileNewName(1);
        //}



        #region FileName

        public string FileName
        {
            get
            {
                var fp = FilePath;

                if (fp.ToCharArray().Any(c => Path.GetInvalidPathChars().Any(s => s == c)))
                    return null;

                return Path.GetFileName(fp);
            }
        }

        #endregion FileName

        #region IsDirty


        bool isDirty = false;
        /// <summary>
        /// IsDirty indicates whether the file currently loaded
        /// in the editor was modified by the user or not.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }

            set
            {
                if (isDirty != value)
                {
                    isDirty = value;

                    OnPropertyChanged(nameof(IsDirty));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        #endregion IsDirty

        #region CanSaveData
        /// <summary>
        /// Get whether edited data can be saved or not.
        /// A type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        public virtual bool CanSaveData { get { return true; } }

        #endregion CanSaveData

        #region Commands

        ICommand closeCommand;

        public virtual ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = CreateCommand(p => OnCloseCommand(),
                                                    p => CanClose());
                }

                return closeCommand;
            }
        }

        public IApplicationViewModel Workspace => _applicationViewModel;

        void OnCloseCommand()
        {
            _applicationViewModel.Close(this);
        }

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder indicated by the path in windows explorer
        /// and select the file (if path points to one).
        /// </summary>
        public ICommand OpenContainingFolderCommand
        {
            get
            {
                return openContainingFolderCommand ?? (openContainingFolderCommand = CreateCommand((p) => OnOpenContainingFolderCommand()));
            }
        }


        /// <summary>
        /// Get CopyFullPathtoClipboard command which will copy
        /// the path of the executable into the windows clipboard.
        /// </summary>
        public ICommand CopyFullPathtoClipboard
        {
            get
            {
                if (copyFullPathtoClipboard == null)
                    copyFullPathtoClipboard = CreateCommand((p) => OnCopyFullPathtoClipboardCommand());

                return copyFullPathtoClipboard;
            }
        }


        #endregion commands

        #endregion properties

        #region methods
        #region abstract methods

        public virtual bool CanSave()
        {
            return true;
        }

        public virtual bool CanSaveAs()
        {
            return CanSave();
        }

        public virtual void ApplySettings()
        {
        }

        public async Task OpenFileAsync(string filePath)
        {
            FilePath = filePath;
            CloseOnErrorWithoutMessage = false;

            State = DocumentState.IsLoading;

            try
            {
                await OpenFileInternal(filePath);
            }
            finally
            {
                // Set this to invalid if viewmodel still things its loading...
                if (State == DocumentState.IsLoading)
                    State = DocumentState.IsInvalid;
            }

            await AfterLoadDocumentInternal();
        }

        async Task OpenFileInternal(string openFilePath)
        {
            try
            {
                var isReal = File.Exists(openFilePath);

                if (!File.Exists(openFilePath))
                    throw new FileNotFoundException(openFilePath);

                FilePath = openFilePath;
                ContentId = filePath;

                try
                {
                    await LoadDocumentInternal(openFilePath);

                    ApplySettings();

                    State = DocumentState.IsEditing;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    IsDirty = false;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, exp);
            }

        }

        protected virtual Task LoadDocumentInternal(string filePath)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterLoadDocumentInternal()
        {
            return Task.CompletedTask;
        }

        public bool SaveFile(string filePath)
        {
            try
            {
                SaveDocumentInternal(filePath);

                // Set new file name in viewmodel and model
                FilePath = filePath;
                ContentId = filePath;

                IsDirty = false;

                return true;
            }
            catch { throw; }
        }

        protected virtual void SaveDocumentInternal(string filePath)
        { }

        /// <summary>
        /// Return the path of the file representation (if any).
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilePath()
        {
            try
            {
                if (File.Exists(FilePath))
                    return Path.GetDirectoryName(FilePath);
            }
            catch
            {
            }

            return string.Empty;
        }

        #endregion abstract methods

        public ProjectInfo GetCurrentProjectInfo()
        {
            var projectPath = _solutionRepository.GetProjectFilePath(filePath);
            var project = _solutionRepository.LoadProjectDocument(projectPath);

            return new ProjectInfo
            {
                Project = project,
                ProjectPath = projectPath,
            };
        }

        /// <summary>
        /// Is executed when the user wants to refresh/re-load
        /// the current content with the currently stored inforamtion.
        /// </summary>
        public virtual async Task ReOpen()
        {
            await OpenFileAsync(FilePath);
        }


        /// <summary>
        /// Get a path that does not represent this document that indicates
        /// a useful alternative representation (eg: StartPage -> Assembly Path).
        /// </summary>
        /// <returns></returns>
        public virtual string GetAlternativePath()
        {
            return FilePath;
        }


        public virtual bool CanClose()
        {
            if (State == DocumentState.IsLoading)
                return false;

            return true;
        }

        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                _clipBoard.SetText(FilePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Opens the folder in which this document is stored in the Windows Explorer.
        /// </summary>
        private void OnOpenContainingFolderCommand()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    var argument = @"/select, " + FilePath;

                    Process.Start("explorer.exe", argument);
                }
                else
                {
                    var parentDir = Directory.GetParent(FilePath).FullName;

                    if (Directory.Exists(parentDir) == false)
                        MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Strings.STR_ACCESS_DIRECTORY_ERROR, parentDir),
                                                        Strings.STR_FILE_FINDING_CAPTION
                                                        );
                    else
                    {
                        string argument = @"/select, " + parentDir;

                        Process.Start("EXPLORER.EXE", argument);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath == null ? string.Empty : FilePath)),
                                                Strings.STR_FILE_FINDING_CAPTION
                                                );
            }
        }

        //protected string GetDefaultFileNewName(int iNewFileCounter,
        //                                       string newDefaultFileName = null,
        //                                       string newDefaultFileExtension = null
        //                                       )
        //{
        //    if (newDefaultFileName != null)
        //        defaultFileName = newDefaultFileName;

        //    if (newDefaultFileExtension != null)
        //        defaultFileType = newDefaultFileExtension;

        //    return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
        //                    defaultFileName,
        //                    (iNewFileCounter == 0 ? string.Empty : " " + iNewFileCounter.ToString()),
        //                    defaultFileType);
        //}


        public override void Dispose()
        {
            base.Dispose();
        }

        //a flag that is true when we load a document for compiling or building
        //when loading a board for build we don't need to update connections(rastnets) or auto compile
        //we might need to repour polygons, though
        public bool LoadedForCompiler { get; set; } = false;

        public abstract IList<IDocumentToolWindow> GetToolWindowsWhenActive();
        public void ShowVisibleToolsFiltered(IList<IDocumentToolWindow> visibleTools)
        {
            //show tools that are needed and set the document
            foreach (var tool in visibleTools)
            {
                tool.SetDocument(this);
                tool.IsVisible = IsToolWindowVisible(tool);
            }
        }

        protected virtual bool IsToolWindowVisible(IDocumentToolWindow toolWindow)
        {
            return true;
        }

        #endregion methods
    }
}
