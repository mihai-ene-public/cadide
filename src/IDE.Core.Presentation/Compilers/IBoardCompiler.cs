using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface IBoardCompiler
{
    Task<CompilerResult> Compile(IBoardDesigner board);
}
