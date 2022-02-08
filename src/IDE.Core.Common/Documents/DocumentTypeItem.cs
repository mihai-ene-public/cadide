namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System.Collections.Generic;

	internal class DocumentTypeItem : IDocumentTypeItem
	{

		/// <summary>
		/// Class constructor
		/// </summary>
		public DocumentTypeItem(string description, IList<string> extensions, int sortPriority = 0)
		{
			Description = description;
			DocFileTypeExtensions = extensions;
			SortPriority = sortPriority;
		}

		public IList<string> DocFileTypeExtensions { get; private set; }

		public string Description { get; private set; }

		public int SortPriority { get; private set; }

	}
}
