using IDE.Core.Storage;
using System;

namespace IDE.Core.Designers
{
    public class NetClassBaseDesignerItem : EditBoxModel
    {
        public string Id { get; set; }

        public NetClassGroupDesignerItem Parent { get; set; }

        public static NetClassBaseDesignerItem LoadFrom(NetClassBaseItem classItem)
        {
            if (classItem is NetClass)
                return NetClassDesignerItem.LoadFrom(classItem as NetClass);
            if (classItem is NetGroup)
                return NetClassGroupDesignerItem.LoadFrom(classItem as NetGroup);

            throw new NotSupportedException("This item is not supported");
        }

        public virtual NetClassBaseItem Save()
        {
            throw new NotImplementedException("Must inherit");
        }
    }
}
