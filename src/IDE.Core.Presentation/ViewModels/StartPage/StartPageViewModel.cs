namespace IDE.Documents.Views
{
    using System;
    using System.Reflection;
    using System.Windows.Input;
    using Core.ViewModels;
    using IDE.Core.Presentation.Resources;
    using Core.Commands;
    using Core.Utilities;
    using IDE.Core.Interfaces;
    using System.Collections.Generic;

    public class StartPageViewModel : FileBaseViewModel, IStartPage
    {
        public const string StartPageContentId = ">StartPage<";

        private readonly IRecentFilesViewModel _recentFilesViewModel;

        public StartPageViewModel() : base()
        {
            this.Title = "Home";
            this.StartPageTip = null;// Strings.STR_STARTPAGE_WELCOME_TT;
            this.ContentId = StartPageContentId;

            filePath = ContentId;
        }

        public StartPageViewModel(IRecentFilesViewModel recentFilesViewModel)
            : this()
        {
            _recentFilesViewModel = recentFilesViewModel;
        }

        #region properties


        #region OpenContainingFolder

        ICommand _openContainingFolderCommand = null;

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder containing the executable in windows explorer
        /// and select the file.
        /// </summary>
        public new ICommand OpenContainingFolderCommand
        {
            get
            {
                if (_openContainingFolderCommand == null)
                    _openContainingFolderCommand = CreateCommand(p => OnOpenContainingFolderCommand());

                return _openContainingFolderCommand;
            }
        }

        private void OnOpenContainingFolderCommand()
        {
            try
            {
                // combine the arguments together it doesn't matter if there is a space after ','
                string argument = @"/select, " + this.GetAlternativePath();

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (System.Exception ex)
            {
                MessageDialog.Show(ex.Message,
                                Strings.STR_FILE_FINDING_CAPTION,
                                XMessageBoxButton.OK, XMessageBoxImage.Error);
            }
        }
        #endregion OpenContainingFolder

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            return new List<IDocumentToolWindow>();
        }

        #region CopyFullPathtoClipboard
        ICommand _copyFullPathtoClipboard = null;

        public new ICommand CopyFullPathtoClipboard
        {
            get
            {
                if (_copyFullPathtoClipboard == null)
                    _copyFullPathtoClipboard = CreateCommand((p) => OnCopyFullPathtoClipboardCommand());

                return _copyFullPathtoClipboard;
            }
        }

        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                clipBoard.SetText(GetAlternativePath());
            }
            catch
            {
            }
        }
        #endregion CopyFullPathtoClipboard

        public IRecentFilesViewModel RecentFilesViewModel => _recentFilesViewModel;

        public string StartPageTip { get; set; }


        public override bool CanSave() { return false; }

        public override bool CanSaveAs() { return false; }

        override public bool CanSaveData => false;

        public override string Title => Strings.STR_STARTPAGE_TITLE;

        #endregion properties

        /// <summary>
        /// Get a path that does not represent this document that indicates
        /// a useful alternative representation (eg: StartPage -> Assembly Path).
        /// </summary>
        /// <returns></returns>
        public override string GetAlternativePath()
        {
            return Assembly.GetEntryAssembly().Location;
        }

    }
}
