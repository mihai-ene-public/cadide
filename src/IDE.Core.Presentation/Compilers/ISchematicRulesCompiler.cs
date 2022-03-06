using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Compilers;

public interface ISchematicRulesCompiler
{
    Task<CompilerResult> Compile(ISchematicDesigner schematic);
}
