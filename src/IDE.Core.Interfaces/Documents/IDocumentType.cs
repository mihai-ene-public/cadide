namespace IDE.Core.Interfaces
{
	using System;
	using System.Collections.Generic;

	public interface IDocumentType
	{
		#region properties
		/// <summary>
		/// Gets a list of file extensions that are supported by this document type.
		/// </summary>
		IList<IDocumentTypeItem> FileTypeExtensions { get; }

		/// <summary>
		/// Gets the default file filter that should be used to save/load a document.
		/// </summary>
		string FileExtension { get; }

		/// <summary>
		/// Gets a string that can be displayed with the DefaultFilter
		/// string in filter drop down section of the file open/save dialog.
		/// </summary>
		string FileFilterName { get; }

		/// <summary>
		/// Gets the key of this document type.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Gets a description that can be displayed for file open/new/save methods.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the actual type of the viewmodel class that implements this document type.
		/// </summary>
		Type DocumentEditorClassType { get; }

        /// <summary>
        /// Document type. Should be a type that inherits LibraryItem 
        /// </summary>
		Type DocumentClassType { get; }

        //Type TypeOfProjectBaseFileRef { get; }

        #endregion properties

        #region methods

        void RegisterFileTypeItem(IDocumentTypeItem fileType);

		/// <summary>
		/// Convinience methode to create an item for the collection of
		/// <seealso cref="IDocumentTypeItem"/> items managed in this class.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="extensions"></param>
		/// <returns></returns>
		IDocumentTypeItem CreateItem(string description, IList<string> extensions);

		void GetFileFilterEntries(IList<IFileFilterEntry> entries);

		#endregion methods
	}
}
