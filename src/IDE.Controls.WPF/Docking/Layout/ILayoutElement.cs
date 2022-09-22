using System.ComponentModel;

namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
{
    ILayoutContainer Parent
    {
        get;
    }
    ILayoutRoot Root
    {
        get;
    }
}
