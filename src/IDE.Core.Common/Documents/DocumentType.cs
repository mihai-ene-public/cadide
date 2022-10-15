namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class manages document specific data items. Such as, filter for file open dialog
    /// </summary>
    internal class DocumentType : IDocumentType
    {
        #region constructors
        public DocumentType(string key,
                            string description,
                            string fileFilterName,
                            string fileExtension,
                            Type documentEditorClassType,
                            Type documentClassType
                            )
        {
            Key = key;
            Description = description;
            FileFilterName = fileFilterName;
            FileExtension = fileExtension;
            DocumentEditorClassType = documentEditorClassType;
            DocumentClassType = documentClassType;
            FileTypeExtensions = null;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a list of file extensions that are supported by this document type.
        /// </summary>
        public IList<IDocumentTypeItem> FileTypeExtensions { get; private set; }

        /// <summary>
        /// Gets the default file filter that should be used to save/load a document.
        /// </summary>
        public string FileExtension { get; private set; }

        /// <summary>
        /// Gets a string that can be displayed with the DefaultFilter
        /// string in filter drop down section of the file open/save dialog.
        /// </summary>
        public string FileFilterName { get; private set; }


        /// <summary>
        /// Gets the key of this document type.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets a description that can be displayed for file open/new/save methods.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the actual type of the viewmodel class that implements this document type.
        /// </summary>
        public Type DocumentEditorClassType { get; private set; }

        public Type DocumentClassType { get; private set; }


        #endregion properties

        #region method
        /// <summary>
        /// Convinience methode to create an item for the collection of
        /// <seealso cref="IDocumentTypeItem"/> items managed in this class.
        /// </summary>
        public IDocumentTypeItem CreateItem(string description, IList<string> extensions)
        {
            return new DocumentTypeItem(description, extensions);
        }

        public void RegisterFileTypeItem(IDocumentTypeItem fileType)
        {
            if (FileTypeExtensions == null)
                FileTypeExtensions = new List<IDocumentTypeItem>();

            FileTypeExtensions.Add(fileType);
        }

        public void GetFileFilterEntries(IList<IFileFilterEntry> entries)
        {
            if (FileTypeExtensions == null)
                return;

            foreach (var item in FileTypeExtensions)
            {
                string ext = string.Empty, ext1 = string.Empty;

                if (item.DocFileTypeExtensions.Count <= 0)
                    continue;

                ext = ext1 = string.Format("*.{0}", item.DocFileTypeExtensions[0].Replace(".", ""));

                for (int i = 1; i < item.DocFileTypeExtensions.Count; i++)
                {
                    ext = string.Format("{0},*.{1}", ext, item.DocFileTypeExtensions[i].Replace(".", ""));
                    ext1 = string.Format("{0};*.{1}", ext1, item.DocFileTypeExtensions[i].Replace(".", ""));
                }

                // log4net XML output (*.log4j,*.log,*.txt,*.xml)|*.log4j;*.log;*.txt;*.xml
                var filterString = new FileFilterEntry(string.Format("{0} ({1}) |{2}",
                                                       item.Description, ext, ext1));

                entries.Add(filterString);
            }
        }
        #endregion method
    }
}
