namespace IDE.Core.ViewModels
{
    using IDE.Core.Interfaces;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// Initialize the AvalonDock Layout. Methods in this class
    /// are called before and after the layout is changed.
    /// </summary>
    public class LayoutInitializer : ILayoutUpdateStrategy
    {
        public LayoutInitializer()
        {
        }

        LayoutAnchorablePane leftPane;
        LayoutAnchorablePane bottomPane;
        LayoutAnchorablePane rightPane;
        LayoutPanel mainPanel;


        public void EnsureLayout(LayoutRoot layout)
        {
            //we'll have all tools visible by default

            layout.RootPanel.Orientation = Orientation.Horizontal;

            if (mainPanel == null)
                mainPanel = new LayoutPanel();
            mainPanel.Orientation = Orientation.Vertical;
            if (!layout.RootPanel.Children.Contains(mainPanel))
                layout.RootPanel.Children.Add(mainPanel);


            var docPaneGroup = layout.RootPanel.Children.OfType<LayoutDocumentPaneGroup>().FirstOrDefault();
            if (docPaneGroup != null)
            {
                layout.RootPanel.Children.Remove(docPaneGroup);
                //layout.RootPanel.Children.Add(mainPanel);
                mainPanel.Children.Add(docPaneGroup);
            }

            //rootpanel doesn't always contains same children; it somehow reposition them
            if (leftPane == null)
            {
                leftPane = new LayoutAnchorablePane();
                //insert 1st

            }
            if (!layout.RootPanel.Children.Contains(leftPane))
                layout.RootPanel.Children.Insert(0, leftPane);

            if (rightPane == null)
            {
                //this should be last
                rightPane = new LayoutAnchorablePane();
            }
            if (!layout.RootPanel.Children.Contains(rightPane))
                layout.RootPanel.Children.Add(rightPane);

            if (bottomPane == null)
            {
                bottomPane = new LayoutAnchorablePane();
            }
            if (!mainPanel.Children.Contains(bottomPane))
                mainPanel.Children.Add(bottomPane);

            //if (!layout.RootPanel.Children.Contains(bottomPane))
            //    layout.RootPanel.Children.Add(bottomPane);

        }

        /// <summary>
        /// Method is called when a completely new layout item is
        /// to be inserted into the current avalondock layout.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="anchorableToShow"></param>
        /// <param name="destinationContainer"></param>
        /// <returns></returns>
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            var tool = anchorableToShow.Content as IToolWindow;

            if (tool != null)
            {
                var preferredLocation = tool.PreferredLocation;

                LayoutGroup<LayoutAnchorable> layoutGroup = FindAnchorableGroup(layout, preferredLocation);

                if (layoutGroup != null)
                {
                    anchorableToShow.CanHide = false;
                    layoutGroup.Children.Add(anchorableToShow);
                }

                return true;
            }

            return false;
        }

        LayoutGroup<LayoutAnchorable> FindAnchorableGroup(LayoutRoot layout, PaneLocation location, bool isVisible = true)
        {
            try
            {
                if (isVisible)
                {
                    EnsureLayout(layout);

                    LayoutAnchorablePane panelGroupParent = null;

                    switch (location)
                    {
                        case PaneLocation.Left:

                            panelGroupParent = leftPane;
                            break;

                        case PaneLocation.Right:
                            panelGroupParent = rightPane;
                            break;

                        case PaneLocation.Bottom:

                            panelGroupParent = bottomPane;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("location:" + location);
                    }

                    return panelGroupParent;
                }
                else
                {
                    LayoutAnchorSide panelGroupParent = null;

                    switch (location)
                    {
                        case PaneLocation.Left:
                            panelGroupParent = layout.LeftSide;
                            break;

                        case PaneLocation.Right:
                            panelGroupParent = layout.RightSide;
                            break;

                        case PaneLocation.Bottom:
                            panelGroupParent = layout.BottomSide;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("location:" + location);
                    }

                    if (panelGroupParent.Children.Count == 0)
                    {
                        var layoutAnchorGroup = new LayoutAnchorGroup();
                        panelGroupParent.Children.Add(layoutAnchorGroup);

                        return layoutAnchorGroup;
                    }
                    else
                    {
                        return panelGroupParent.Children[0];
                    }
                }
            }
            catch //(Exception exp)
            {
               // logger.Error(exp);
            }

            return null;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            // If this is the first anchorable added to this pane, then use the preferred size.
            var tool = anchorableShown.Content as IToolWindow;
            if (tool != null)
            {
                var anchorablePane = anchorableShown.Parent as LayoutAnchorablePane;
                //var anchorablePane = anchorableShown.Parent as LayoutAnchorGroup;
                if (anchorablePane != null && anchorablePane.ChildrenCount == 1)
                {
                    //anchorablePane.IsVisible = true;
                    switch (tool.PreferredLocation)
                    {
                        case PaneLocation.Left:
                        case PaneLocation.Right:
                            anchorablePane.DockWidth = new GridLength(tool.PreferredWidth, GridUnitType.Pixel);
                            break;
                        case PaneLocation.Bottom:
                            anchorablePane.DockHeight = new GridLength(tool.PreferredHeight, GridUnitType.Pixel);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }
}
