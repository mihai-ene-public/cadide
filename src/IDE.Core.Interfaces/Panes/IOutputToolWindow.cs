namespace IDE.Core.Interfaces;

public interface IOutputToolWindow : IToolWindow, IRegisterable
{
    void AppendLine(string text);
    void Append(string text);
    void Clear();
}
