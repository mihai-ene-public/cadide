namespace IDE.Core.Interfaces
{
    using System;
    using System.Windows.Input;
    using System.Windows;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using IDE.Core.Types.Media;

    /// <summary>
    /// Inteface that is supported by document related viewmodels.
    /// </summary>
    public interface IFileBaseViewModel : ILayoutItem,
                                          IDisposable
    {

        #region properties

        /// <summary>
        /// Gets the current state of the document. States may differ during
        /// initialization, loading, and other asynchron processings...
        /// </summary>
        DocumentState State { get; set; }

        /// <summary>
        /// File path of the current document.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets/sets whether a given document has been changed since loading
        /// from filesystem, or not.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Gets the currently assigned name of the file in the file system.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Get whether edited data can be saved or not.
        /// A type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        bool CanSaveData { get; }

        ICommand CloseCommand { get; }


        bool LoadedForCompiler { get; set; }

        #endregion properties

        #region methods
        /// <summary>
        /// Is executed when the user wants to refresh/re-load
        /// the current content with the currently stored inforamtion.
        /// </summary>
        Task ReOpen();

        /// <summary>
        /// Indicate whether document can be closed or not.
        /// </summary>
        /// <returns></returns>
        bool CanClose();

        /// <summary>
        /// Indicate whether document can be saved in the currennt state.
        /// </summary>
        /// <returns></returns>
        bool CanSave();

        /// <summary>
        /// Indicate whether document can be saved as.
        /// </summary>
        /// <returns></returns>
        bool CanSaveAs();

        ProjectInfo GetCurrentProjectInfo();

        Task OpenFileAsync(string filePath);

        /// <summary>
        /// Save this document as.
        /// </summary>
        /// <returns></returns>
        bool SaveFile(string filePath);

        string GetFilePath();

        void ApplySettings();


        /// <summary>
        /// returns a list of tool windows that needs to be visible when this document is active
        /// </summary>
        /// <returns></returns>
        IList<IDocumentToolWindow> GetToolWindowsWhenActive();

        void ShowVisibleToolsFiltered(IList<IDocumentToolWindow> visibleTools);

        #endregion methods
    }


}
