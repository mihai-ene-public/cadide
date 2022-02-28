namespace IDE.Core.Interfaces
{
	using IDE.Core.Interfaces;

	/// <summary>
	/// A document parent is a viewmodel that holds the collection of documents
	/// and can inform other objects when the active document changes.
	/// </summary>
	public interface IDocumentParent
	{

		IFileBaseViewModel ActiveDocument
		{
			get;
			set;
		}
	}
}
