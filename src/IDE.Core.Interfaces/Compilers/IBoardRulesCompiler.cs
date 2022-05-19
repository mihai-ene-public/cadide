using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Interfaces.Compilers;

public interface IBoardRulesCompiler
{
    Task<CompilerResult> Compile(IBoardDesigner board);

    double GetElectricalClearance(IBoardDesigner board, ISelectableItem item1, ISelectableItem item2, double defaultClearance = 0.254);
}
