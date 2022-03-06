using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface ISchematicCompiler
{
    Task<CompilerResult> Compile(ISchematicDesigner schematic);
}
