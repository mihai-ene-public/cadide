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

		IDocumentType FindDocumentTypeByExtension(string fileExtension);

		IFileFilterEntries GetFileFilterEntries(string key = "");
	}
}
