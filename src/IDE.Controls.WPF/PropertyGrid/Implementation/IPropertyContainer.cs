using System.ComponentModel;
using System.Windows;

namespace IDE.Controls.WPF.PropertyGrid;

internal interface IPropertyContainer: INotifyPropertyChanged
{
    PropertyContainerHelper ContainerHelper { get; }

    bool? IsPropertyVisible(PropertyDescriptor pd);

}
