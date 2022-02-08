namespace IDE.Core.Documents
{
    using IDE.Core.Interfaces;
    using System.Collections.Generic;

	internal class FileFilterEntries : IFileFilterEntries
	{
		#region fields
		private readonly List<IFileFilterEntry> filterEntries;
		#endregion fields

		#region contructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileFilterEntries(List<IFileFilterEntry> entries)
		{
			filterEntries = entries;
		}
		#endregion contructors

		#region properties
		#endregion properties

		#region methods
		/// <summary>
		/// Gets a string that can be used as filter selection in a file open dialog.
		/// </summary>
		/// <returns></returns>
		public string GetFilterString()
		{
			var s = string.Empty;
			var d = string.Empty;
			foreach (var item in this.filterEntries)
			{
				s = s + d + item.FileFilter;
				d = "|";
			}

			return s;
		}


        public IDocumentType GetFileDocumentType(int index)
        {
            return filterEntries[index].DocumentType;
        }

		#endregion methods
	}
}
