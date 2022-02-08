using IDE.Core.Storage;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDE.Core.Designers
{
    /// <summary>
    /// a group for classes and other groups on schematic
    /// </summary>
    public class NetClassGroupDesignerItem : NetClassBaseDesignerItem
    {

        public ObservableCollection<NetClassBaseDesignerItem> Children { get; set; } = new ObservableCollection<NetClassBaseDesignerItem>();

        public void AddChild(NetClassBaseDesignerItem newChild)
        {
            newChild.Parent = this;
            Children.Add(newChild);
        }

        public void RemoveChild(NetClassBaseDesignerItem child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        /// <summary>
        /// loads recursively a group
        /// </summary>
        public static NetClassGroupDesignerItem LoadFrom(NetGroup netGroup)
        {
            var thisGroup = new NetClassGroupDesignerItem { Id = netGroup.Id, Name = netGroup.Name };

            //load children recursive
            foreach (var netChild in netGroup.Children)
                thisGroup.AddChild(NetClassBaseDesignerItem.LoadFrom(netChild));

            return thisGroup;

        }

        /// <summary>
        /// saves a group recursively
        /// </summary>
        public override NetClassBaseItem Save()
        {
            var ng = new NetGroup { Id = Id, Name = Name };
            ng.Children.AddRange(Children.Select(c => c.Save()));

            return ng;
        }
    }
}
