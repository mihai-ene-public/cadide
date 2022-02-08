using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    //holds a list of nets that are unique by name
    //when adding a new net the one with the lowest id is kept
    public class SchematicNetManager : UniqueNameManager<INet>, INetManager
    {
        protected override string GetPrefix()
        {
            return "Net$";
        }

        protected override INet CreateInstance()
        {
            return new SchematicNet();
        }
    }
}
