using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class SchematicBusManager : UniqueNameManager<IBus>, IBusManager
    {
        protected override string GetPrefix()
        {
            return "Bus$";
        }

        protected override IBus CreateInstance()
        {
            return new SchematicBus();
        }
    }
}
