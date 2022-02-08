namespace IDE.Core.Interfaces
{
    public interface IDocumentToolWindow : IToolWindow
    {
        void SetDocument(IFileBaseViewModel document);
    }
}
