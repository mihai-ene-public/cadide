using System.ComponentModel;

namespace IDE.Controls.WPF.Core.Extensions;

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
