using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Builders
{
    public interface ISchematicBuilder
    {
        Task<BuildResult> Build(ISchematicDesigner schematic);
    }
}
