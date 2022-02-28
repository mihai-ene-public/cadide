using IDE.Core.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// This interface models the viewmodel that manages the complete
    /// application life cyle from start to end. It publishes the methodes,
    /// properties, and events necessary to integrate the application into
    /// a given shell (BootStrapper, App.xaml.cs etc).
    /// </summary>
    public interface IApplicationViewModel
    {
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        event EventHandler RequestClose;

        event EventHandler RequestExit;

        /// <summary>
        /// raised when selection in canvas was changed
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// raised when highlight in canvas was changed
        /// </summary>
        event EventHandler HighlightChanged;

        //request to load layout on solution open
        event EventHandler<string> LoadLayoutRequested;

        void OnSelectionChanged(object sender, EventArgs e);

        void OnHighlightChanged(object sender, EventArgs e);

        /// <summary>
        /// Get/set property to determine whether window is in maximized state or not.
        /// (this can be handy to determine when a resize grip should be shown or not)
        /// </summary>
        bool? IsNotMaximized { get; set; }

        /// <summary>
        /// Gets/sets whether the current application shut down process
        /// is cancelled or not.
        /// </summary>
        bool ShuttingDownCancel { get; set; }

        IFileBaseViewModel ActiveDocument { get; }

        IList<IFileBaseViewModel> Files { get; }

        IList<IToolWindow> Tools { get; }

        void ShowStartPage();

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        void OnRequestClose();

        /// <summary>
        /// Save session data on closing
        /// </summary>
        void OnClosing(object sender, CancelEventArgs e);

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        bool CanCloseAndSaved();

        void CreateAppDataFolder();

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        void LoadConfig();

        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        void SaveConfig();

        void SaveXmlLayout(string xmlLayout);

        /// <summary>
        /// Bind a window to some commands to be executed by the viewmodel.
        /// </summary>
        void InitCommandBinding(ILayoutableWindow window);

        //when opening a solution LoadLayoutRequested event is raised
        Task OpenSolution(string filePath);

        Task<IFileBaseViewModel> Open(ISolutionExplorerNodeModel item, string filePath);

        Task<IFileBaseViewModel> OpenDocumentAsync(ISolutionExplorerNodeModel item, bool loadedForCompiler = false);

        bool Close(IFileBaseViewModel file);

    }

}
