namespace IDE.Core.Interfaces;

public interface IOutputToolWindow : IToolWindow
{
    void AppendLine(string text);
    void Append(string text);
    void Clear();
}
