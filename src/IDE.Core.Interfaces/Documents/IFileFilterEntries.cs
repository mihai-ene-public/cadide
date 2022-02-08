namespace IDE.Core.Interfaces
{
	public interface IFileFilterEntries
	{
		string GetFilterString();

        IDocumentType GetFileDocumentType(int index);
        //FileOpenDelegate GetFileOpenMethod(int idx);
    }
}
