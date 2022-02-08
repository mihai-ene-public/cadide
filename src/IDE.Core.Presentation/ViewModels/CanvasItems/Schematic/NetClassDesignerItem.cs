using IDE.Core.Storage;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// represents a net class on schematic
    /// </summary>
    public class NetClassDesignerItem : NetClassBaseDesignerItem
    {
        public static NetClassDesignerItem LoadFrom(NetClass netClass)
        {
            return new NetClassDesignerItem { Id = netClass.Id, Name = netClass.Name };
        }

        public override NetClassBaseItem Save()
        {
            return new NetClass { Id = Id, Name = Name };
        }
    }
}
