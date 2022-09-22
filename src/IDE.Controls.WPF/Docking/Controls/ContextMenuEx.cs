using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Controls.WPF.Docking.Controls;

public class ContextMenuEx : ContextMenu
{
    #region Constructors

    static ContextMenuEx()
    {
    }

    public ContextMenuEx()
    {
    }

    #endregion

    #region Overrides

    protected override System.Windows.DependencyObject GetContainerForItemOverride()
    {
        return new MenuItemEx();
    }

    protected override void OnOpened(System.Windows.RoutedEventArgs e)
    {
        BindingOperations.GetBindingExpression(this, ItemsSourceProperty).UpdateTarget();

        base.OnOpened(e);
    }

    #endregion
}
