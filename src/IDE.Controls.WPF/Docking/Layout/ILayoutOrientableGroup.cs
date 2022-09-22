using System.Windows.Controls;

namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutOrientableGroup : ILayoutGroup
{
    Orientation Orientation
    {
        get; set;
    }
}
