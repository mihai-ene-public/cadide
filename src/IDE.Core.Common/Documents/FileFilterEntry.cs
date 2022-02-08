namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System;

	internal class FileFilterEntry : IFileFilterEntry
	{
		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="fileFilter"></param>
		/// <param name="fileOpenMethod"></param>
		public FileFilterEntry(string fileFilter)
		{
			FileFilter = fileFilter;
		}
		#endregion constructors

		#region properties

		public string FileFilter { get; private set; }

        public IDocumentType DocumentType { get; set; }

		#endregion properties
	}
}
