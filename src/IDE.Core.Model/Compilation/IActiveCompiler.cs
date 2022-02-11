using IDE.Core.Interfaces;
using System.Threading.Tasks;

namespace IDE.Core.Compilation
{
    public interface IActiveCompiler
    {
        Task Compile(IFileBaseViewModel file);
    }
}