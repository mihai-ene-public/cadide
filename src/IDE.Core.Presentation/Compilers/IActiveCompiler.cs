using IDE.Core.Interfaces;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.Compilers;

public interface IActiveCompiler
{
    Task Compile(IFileBaseViewModel file);
}
