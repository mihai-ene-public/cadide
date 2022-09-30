namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class DocumentTypeManager : IDocumentTypeManager
    {
        private readonly SortableObservableCollection<IDocumentType> documentTypes = new SortableObservableCollection<IDocumentType>();

        public DocumentTypeManager()
        {
        }

        public IList<IDocumentType> DocumentTypes
        {
            get { return documentTypes; }
        }

        /// <summary>
        /// Method can be invoked in PRISM MEF module registration to register a new document (viewmodel)
        /// type and its default file extension. The result of this call is an <seealso cref="IDocumentType"/>
        /// object and a <seealso cref="RegisterDocumentTypeEvent"/> event to inform listers about the new
        /// arrival of the new document type.
        /// </summary>
        public IDocumentType RegisterDocumentType(string Key,
                                                   string description,
                                                   string fileFilterName,
                                                   string fileExtension,
                                                   Type typeOfDocument
                                                 )
        {
            var newFileType = new DocumentType(Key, description, fileFilterName, fileExtension, typeOfDocument);

            documentTypes.Add(newFileType);
            documentTypes.Sort(i => i.FileFilterName, System.ComponentModel.ListSortDirection.Ascending);

            return newFileType;
        }

        /// <summary>
        /// Finds a document type that can handle a file
        /// with the given file extension eg ".txt" or "txt"
        /// when the original file name was "Readme.txt".
        /// 
        /// Always returns the 1st document type handler that matches the extension.
        /// </summary>
        public IDocumentType FindDocumentTypeByExtension(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return null;

            var idx = fileExtension.LastIndexOf(".");

            if (idx >= 0)
                fileExtension = fileExtension.Substring(idx + 1);

            var ret = documentTypes.FirstOrDefault(d => d.FileExtension == fileExtension);

            return ret;
        }

        /// <summary>
        /// Goes through all file/document type definitions and returns a filter string
        /// object that can be used in conjunction with FileOpen and FileSave dialog filters.
        /// </summary>
        public IFileFilterEntries GetFileFilterEntries(string key = "")
        {
            var fileEntries = new List<IFileFilterEntry>();

            if (documentTypes != null)
            {
                foreach (var item in documentTypes)
                {
                    if (key == string.Empty || key == item.Key)
                    {
                        var filter = item.FileExtension.Replace(".", "");

                        // format filter entry like "Structured Query Language (*.sql) |*.sql"
                        var s = new FileFilterEntry(string.Format("{0} (*.{1}) |*.{2}",
                                                                    item.FileFilterName, filter, filter)
                                                                    );
                        s.DocumentType = item;
                        fileEntries.Add(s);

                        // Add all file sub-filters for this viewmodel class
                        item.GetFileFilterEntries(fileEntries);
                    }
                }
            }

            var list = fileEntries.OrderBy(f => f.FileFilter).ToList();

            return new FileFilterEntries(list);
        }
    }
}
