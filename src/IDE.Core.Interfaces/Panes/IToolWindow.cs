namespace IDE.Core.Interfaces
{

    public interface IToolWindow : ILayoutItem
    {
        string Name { get; }

        bool IsVisible { get; set; }

        bool IsActive { get; set; }

        bool CanHide { get; set; }

        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }


    }
}
