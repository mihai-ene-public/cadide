using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels;

public interface ISolutionExplorerToolWindow : IToolWindow
{
    Task OpenFile(string filePath);
}
