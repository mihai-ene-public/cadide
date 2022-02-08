namespace IDE.Core.Interfaces
{
    using System.IO;

    public interface IOutput : IToolWindow
    {
        void AppendLine(string text);
        void Append(string text);
        void Clear();
    }
}