using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace IDE.Controls.WPF.Docking.Controls;

public class LayoutGridResizerControl : Thumb
{
    #region Constructors

    static LayoutGridResizerControl()
    {
        //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        //This style is defined in themes\generic.xaml
        DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(typeof(LayoutGridResizerControl)));
        HorizontalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        VerticalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        BackgroundProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Transparent));
        IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(true, null));
    }

    #endregion

    #region Properties

    #region BackgroundWhileDragging

    /// <summary>
    /// BackgroundWhileDragging Dependency Property
    /// </summary>
    public static readonly DependencyProperty BackgroundWhileDraggingProperty = DependencyProperty.Register("BackgroundWhileDragging", typeof(Brush), typeof(LayoutGridResizerControl),
            new FrameworkPropertyMetadata((Brush)Brushes.Black));

    /// <summary>
    /// Gets or sets the BackgroundWhileDragging property.  This dependency property 
    /// indicates ....
    /// </summary>
    public Brush BackgroundWhileDragging
    {
        get
        {
            return (Brush)GetValue(BackgroundWhileDraggingProperty);
        }
        set
        {
            SetValue(BackgroundWhileDraggingProperty, value);
        }
    }

    #endregion

    #region OpacityWhileDragging

    /// <summary>
    /// OpacityWhileDragging Dependency Property
    /// </summary>
    public static readonly DependencyProperty OpacityWhileDraggingProperty = DependencyProperty.Register("OpacityWhileDragging", typeof(double), typeof(LayoutGridResizerControl),
            new FrameworkPropertyMetadata((double)0.5));

    /// <summary>
    /// Gets or sets the OpacityWhileDragging property.  This dependency property 
    /// indicates ....
    /// </summary>
    public double OpacityWhileDragging
    {
        get
        {
            return (double)GetValue(OpacityWhileDraggingProperty);
        }
        set
        {
            SetValue(OpacityWhileDraggingProperty, value);
        }
    }

    #endregion

    #endregion
}
