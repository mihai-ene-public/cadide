namespace IDE.Controls.WPF.Docking.Layout;

public interface ILayoutUpdateStrategy
{
    bool BeforeInsertAnchorable(
        LayoutRoot layout,
        LayoutAnchorable anchorableToShow,
        ILayoutContainer destinationContainer);

    void AfterInsertAnchorable(
        LayoutRoot layout,
        LayoutAnchorable anchorableShown);


    bool BeforeInsertDocument(
        LayoutRoot layout,
        LayoutDocument anchorableToShow,
        ILayoutContainer destinationContainer);

    void AfterInsertDocument(
        LayoutRoot layout,
        LayoutDocument anchorableShown);
}
