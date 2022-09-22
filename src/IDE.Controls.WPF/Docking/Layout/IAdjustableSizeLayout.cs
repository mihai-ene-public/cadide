using System.Windows;

namespace IDE.Controls.WPF.Docking.Layout;

public interface IAdjustableSizeLayout
{
    void AdjustFixedChildrenPanelSizes(Size? parentSize = null);
}
