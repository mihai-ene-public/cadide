namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutPane : ILayoutContainer, ILayoutElementWithVisibility
{
    void MoveChild(int oldIndex, int newIndex);

    void RemoveChildAt(int childIndex);
}
