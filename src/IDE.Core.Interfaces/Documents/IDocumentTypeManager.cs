namespace IDE.Core.Interfaces
{
	using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
	using IDE.Core.Interfaces;

	/// <summary>
	/// Interface specification for the document management service that drives
	/// creation, loading and saving of documents in the low level backend.
	/// </summary>
	public interface IDocumentTypeManager
	{
		IList<IDocumentType> DocumentTypes { get; }

		
		IDocumentType RegisterDocumentType(string Key,
										   string description,
										   string fileFilterName,
										   string fileExtension, 
										   Type typeOfDocument
										   );

		/// <summary>
		/// Finds a document type that can handle a file
		/// with the given file extension eg ".txt" or "txt"
		/// when the original file name was "Readme.txt".
		/// Always returns the 1st document type handler that matches the extension.
		/// </summary>
		IDocumentType FindDocumentTypeByExtension(string fileExtension, bool trimPeriod = false);

		/// <summary>
		/// Goes through all file/document type definitions and returns a filter string
		/// object that can be used in conjunction with FileOpen and FileSave dialog filters.
		/// </summary>
		/// <param name="key">Get entries for this viewmodel only,
		/// or all entries if key parameter is not set.</param>
		/// <returns></returns>
		IFileFilterEntries GetFileFilterEntries(string key = "");
	}
}
