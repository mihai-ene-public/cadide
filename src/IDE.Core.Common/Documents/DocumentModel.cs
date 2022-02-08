namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System;

    /// <summary>
    /// Class models the basic properties and behaviours of a low level file stored on harddisk.
    /// </summary>
    public class DocumentModel : IDocumentModel, IDisposable
    {
        #region fields

        private FileName fileName;

        private FileChangeWatcher fileChangeWatcher = null;

        #endregion fields

        #region constructors

        public DocumentModel()
        {
            SetDefaultDocumentModel();
        }

        
        #endregion constructors

        /// <summary>
        /// Occurs when the file name has changed.
        /// </summary>
        public event EventHandler FileNameChanged;

        #region properties
        /// <summary>
        /// Gets whether the file content on storake (harddisk) can be changed
        /// or whether file is readonly through file properties.
        /// </summary>
        public bool IsReadonly { get; private set; }

        /// <summary>
        /// Determines whether a document has ever been stored on disk or whether
        /// the current path and other file properties are currently just initialized
        /// in-memory with defaults.
        /// </summary>
        public bool IsReal { get; private set; }

        /// <summary>
        /// Gets the complete path and file name for this document.
        /// </summary>
        public string FileNamePath
        {
            get
            {
                if (fileName == null)
                    return null;

                return fileName.ToString();
            }
        }

        /// <summary>
        /// Gets the name of a file.
        /// </summary>
        public string FileName
        {
            get
            {
                if (fileName == null)
                    return null;

                return System.IO.Path.GetFileName(FileNamePath);
            }
        }

        /// <summary>
        /// Gets the path of a file.
        /// </summary>
        public string Path
        {
            get
            {
                return System.IO.Path.GetFullPath(this.FileNamePath);
            }
        }

        /// <summary>
        /// Gets the file extension of the document represented by this path.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return fileName.GetExtension();
            }
        }

        public bool WasChangedExternally
        {
            get
            {
                if (fileChangeWatcher == null)
                    return false;

                return fileChangeWatcher.WasChangedExternally;
            }

            set
            {
                if (fileChangeWatcher == null)
                    return;

                if (fileChangeWatcher.WasChangedExternally != value)
                    fileChangeWatcher.WasChangedExternally = value;
            }
        }
        #endregion properties

        #region methods
        protected virtual void ChangeFileName(FileName newValue)
        {
            ////SD.MainThread.VerifyAccess();

            //// Already done by caller
            ////this.fileName = newValue;

            if (FileNameChanged != null)
                FileNameChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Assigns a filename and path to this document model. This will also
        /// refresh all properties (IsReadOnly etc..) that can be queried for this document.
        /// </summary>
        /// <param name="fileNamePath"></param>
        /// <param name="isReal">Determines whether file exists on disk
        /// (file open -> properties are refreshed from persistence) or not
        /// (properties are reset to default).</param>
        public void SetFileNamePath(string fileNamePath, bool isReal)
        {
            if (fileNamePath != null)
                fileName = new FileName(fileNamePath);

            IsReal = isReal;

            if (IsReal && fileNamePath != null)
            {
                QueryFileProperies();
                ChangeFileName(fileName);
            }
        }

        /// <summary>
        /// Resets the IsReal property to adjust model when a new document has been saved
        /// for the very first time.
        /// </summary>
        /// <param name="IsReal">Determines whether file exists on disk
        /// (file open -> properties are refreshed from persistence) or not
        /// (properties are reset to default).</param>
        public void SetIsReal(bool isReal)
        {
            IsReal = isReal;

            if (IsReal)
            {
                if (fileChangeWatcher == null)
                    QueryFileProperies();
            }
        }

        /// <summary>
        /// Query sub-system for basic properties if this file is supposed to exist in persistence.
        /// </summary>
        private void QueryFileProperies()
        {
            try
            {
                if (IsReal)
                {
                    System.IO.FileInfo f = new System.IO.FileInfo(FileNamePath);
                    IsReadonly = f.IsReadOnly;

                    if (fileChangeWatcher != null)
                    {
                        fileChangeWatcher.Dispose();
                        fileChangeWatcher = null;
                    }

                    fileChangeWatcher = new FileChangeWatcher(this);
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error in QueryFileProperies", exp);
            }
        }

        /// <summary>
        /// Set a file specific value to determine whether file
        /// watching is enabled/disabled for this file.
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public bool EnableDocumentFileWatcher(bool isEnabled)
        {
            try
            {
                if (isEnabled == true)
                {
                    // Enable file watcher for this file
                    if (IsReal == false)
                        return false;

                    if (fileChangeWatcher == null)
                        QueryFileProperies();

                    if (fileChangeWatcher == null)
                        return false;

                    if (fileChangeWatcher.Enabled == false)
                        fileChangeWatcher.Enabled = true;

                    return true;
                }
                else
                {
                    // Disable file watcher for this file
                    if (fileChangeWatcher == null)
                        return false;

                    if (fileChangeWatcher.Enabled)
                        fileChangeWatcher.Enabled = false;

                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Resets all document property values to their defaults.
        /// </summary>
        private void SetDefaultDocumentModel()
        {
           IsReadonly = true;
           IsReal = false;
           fileName = null;
        }

        public void Dispose()
        {
            if (fileChangeWatcher != null)
            {
                fileChangeWatcher.Dispose();
                fileChangeWatcher = null;
            }
        }
        #endregion methods
    }
}
