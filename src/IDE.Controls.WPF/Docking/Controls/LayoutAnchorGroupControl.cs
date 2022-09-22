using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IDE.Controls.WPF.Docking.Layout;

namespace IDE.Controls.WPF.Docking.Controls;

public class LayoutAnchorGroupControl : Control, ILayoutControl
{
    #region Members

    private ObservableCollection<LayoutAnchorControl> _childViews = new ObservableCollection<LayoutAnchorControl>();
    private LayoutAnchorGroup _model;

    #endregion

    #region Constructors

    static LayoutAnchorGroupControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorGroupControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorGroupControl)));
    }

    internal LayoutAnchorGroupControl(LayoutAnchorGroup model)
    {
        _model = model;
        CreateChildrenViews();

        _model.Children.CollectionChanged += (s, e) => OnModelChildrenCollectionChanged(e);
    }

    #endregion

    #region Properties

    public ObservableCollection<LayoutAnchorControl> Children
    {
        get
        {
            return _childViews;
        }
    }

    public ILayoutElement Model
    {
        get
        {
            return _model;
        }
    }

    #endregion

    #region Private Methods

    private void CreateChildrenViews()
    {
        var manager = _model.Root.Manager;
        foreach (var childModel in _model.Children)
        {
            _childViews.Add(new LayoutAnchorControl(childModel) { Template = manager.AnchorTemplate });
        }
    }

    private void OnModelChildrenCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
        {
            if (e.OldItems != null)
            {
                {
                    foreach (var childModel in e.OldItems)
                        _childViews.Remove(_childViews.First(cv => cv.Model == childModel));
                }
            }
        }

        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            _childViews.Clear();

        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
        {
            if (e.NewItems != null)
            {
                var manager = _model.Root.Manager;
                int insertIndex = e.NewStartingIndex;
                foreach (LayoutAnchorable childModel in e.NewItems)
                {
                    _childViews.Insert(insertIndex++, new LayoutAnchorControl(childModel) { Template = manager.AnchorTemplate });
                }
            }
        }
    }

    #endregion
}
