namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// </summary>
   // [Export(typeof(IDocumentTypeManager))]
    public class DocumentTypeManager : IDocumentTypeManager
    {
        #region fields
        private readonly SortableObservableCollection<IDocumentType> documentTypes = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentTypeManager()
        {
            documentTypes = new SortableObservableCollection<IDocumentType>(new List<IDocumentType>());
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a collection of document types that are supported by this application.
        /// </summary>
        public IList<IDocumentType> DocumentTypes
        {
            get { return documentTypes; }
        }
        #endregion properties

        #region Methods
        /// <summary>
        /// Method can be invoked in PRISM MEF module registration to register a new document (viewmodel)
        /// type and its default file extension. The result of this call is an <seealso cref="IDocumentType"/>
        /// object and a <seealso cref="RegisterDocumentTypeEvent"/> event to inform listers about the new
        /// arrival of the new document type.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Description"></param>
        /// <param name="FileFilterName"></param>
        /// <param name="DefaultFilter"></param>
        /// <param name="FileOpenMethod"></param>
        /// <param name="CreateDocumentMethod"></param>
        /// <param name="typeOfDocument"></param>
        /// <param name="sortPriority"></param>
        /// <returns></returns>
        public IDocumentType RegisterDocumentType(string Key,
                                                  string Description,
                                                  string FileFilterName,
                                                  string DefaultFilter,
                                                  Type typeOfDocument
                                                  //Type typeOfProjectBaseFileRef
                                                 )
        {
            var newFileType = new DocumentType(Key, Description, FileFilterName, DefaultFilter, typeOfDocument);

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
        /// <param name="fileExtension"></param>
        /// <param name="trimPeriod">Determines if an additional '.' character is removed
        /// from the given extension string or not.</param>
        /// <returns></returns>
        public IDocumentType FindDocumentTypeByExtension(string fileExtension,
                                                         bool trimPeriod = false)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return null;

            if (trimPeriod)
            {
                int idx;

                if ((idx = fileExtension.LastIndexOf(".")) >= 0)
                    fileExtension = fileExtension.Substring(idx + 1);
            }

            if (string.IsNullOrEmpty(fileExtension) == true)
                return null;

            var ret = documentTypes.FirstOrDefault(d => d.DefaultFilter == fileExtension);

            return ret;
        }

        /// <summary>
        /// Find a document type based on its key.
        /// </summary>
        /// <param name="typeOfDoc"></param>
        /// <returns></returns>
        public IDocumentType FindDocumentTypeByKey(string typeOfDoc)
        {
            if (string.IsNullOrEmpty(typeOfDoc) == true)
                return null;

            return documentTypes.FirstOrDefault(d => d.Key == typeOfDoc);
        }

        /// <summary>
        /// Goes through all file/document type definitions and returns a filter string
        /// object that can be used in conjunction with FileOpen and FileSave dialog filters.
        /// </summary>
        /// <param name="key">Get entries for this viewmodel only,
        /// or all entries if key parameter is not set.</param>
        /// <returns></returns>
        public IFileFilterEntries GetFileFilterEntries(string key = "")
        {
            //var ret = new SortedList<int, IFileFilterEntry>();
            var fileEntries = new List<IFileFilterEntry>();

            if (documentTypes != null)
            {
                foreach (var item in documentTypes)
                {
                    if (key == string.Empty || key == item.Key)
                    {
                        var filter = item.DefaultFilter.Replace(".", "");

                        // format filter entry like "Structured Query Language (*.sql) |*.sql"
                        var s = new FileFilterEntry(string.Format("{0} (*.{1}) |*.{2}",
                                                                    item.FileFilterName, filter, filter)
                                                                    );//item.FileOpenMethod);
                        s.DocumentType = item;
                        //ret.Add(item.SortPriority, s);
                        fileEntries.Add(s);

                        // Add all file sub-filters for this viewmodel class
                        item.GetFileFilterEntries(fileEntries);//, item.FileOpenMethod);
                    }
                }
            }

            //List<IFileFilterEntry> list = new List<IFileFilterEntry>();

            //foreach (var item in ret)
            //    list.Add(item.Value);
            var list = fileEntries.OrderBy(f => f.FileFilter).ToList();

            return new FileFilterEntries(list);
        }
        #endregion Methods
    }
}
