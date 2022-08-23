using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Controls.WPF.Core.Extensions;
internal static class PropertyDescriptorExtensions
{
    internal static T GetAttribute<T>(this PropertyDescriptor property) where T : Attribute
    {
        return property.Attributes.OfType<T>().FirstOrDefault();
    }

}

internal static class PropertyChangedExtensions
{
    internal static void OnPropertyChanged(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string propertyName)
    {
        if (handler != null)
        {
            handler(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
