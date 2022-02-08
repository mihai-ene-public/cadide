namespace IDE.Core.Interfaces
{
	using System;
	using IDE.Core.Interfaces;

	public interface IFileFilterEntry
	{
		string FileFilter { get; }


        IDocumentType DocumentType { get; set; }
        //FileOpenDelegate FileOpenMethod { get; }
    }
}
