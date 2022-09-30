using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using IDE.Controls.WPF.Core.Utilities;

namespace IDE.Controls.WPF;
public class AdvancedTextBox : TextBox
{
    static AdvancedTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedTextBox), new FrameworkPropertyMetadata(typeof(AdvancedTextBox)));
    }
    public AdvancedTextBox()
    {
    }

    public bool AutoSelectEnabled
    {
        get
        {
            return (bool)GetValue(AutoSelectEnabledProperty);
        }
        set
        {
            SetValue(AutoSelectEnabledProperty, value);
        }
    }

    public static readonly DependencyProperty AutoSelectEnabledProperty =
                    DependencyProperty.Register("AutoSelectEnabled",
                                                 typeof(bool),
                                                 typeof(AdvancedTextBox),
                                                 new UIPropertyMetadata(true));

    #region KeepWatermarkOnGotFocus

    public static readonly DependencyProperty KeepWatermarkOnGotFocusProperty = DependencyProperty.Register("KeepWatermarkOnGotFocus", typeof(bool), typeof(AdvancedTextBox), new UIPropertyMetadata(false));
    public bool KeepWatermarkOnGotFocus
    {
        get
        {
            return (bool)GetValue(KeepWatermarkOnGotFocusProperty);
        }
        set
        {
            SetValue(KeepWatermarkOnGotFocusProperty, value);
        }
    }

    #endregion KeepWatermarkOnGotFocus

    #region Watermark

    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(AdvancedTextBox), new UIPropertyMetadata(null));
    public object Watermark
    {
        get
        {
            return GetValue(WatermarkProperty);
        }
        set
        {
            SetValue(WatermarkProperty, value);
        }
    }

    #endregion Watermark

    #region WatermarkTemplate

    public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(AdvancedTextBox), new UIPropertyMetadata(null));
    public DataTemplate WatermarkTemplate
    {
        get
        {
            return (DataTemplate)GetValue(WatermarkTemplateProperty);
        }
        set
        {
            SetValue(WatermarkTemplateProperty, value);
        }
    }

    #endregion WatermarkTemplate


    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnPreviewGotKeyboardFocus(e);

        if (AutoSelectEnabled)
        {
            // If the focus was not in one of our child ( or popup ), we select all the text.
            if (!TreeHelper.IsDescendantOf(e.OldFocus as DependencyObject, this))
            {
                SelectAll();
            }
        }
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);

        if (!AutoSelectEnabled)
            return;

        if (IsKeyboardFocusWithin == false)
        {
            Focus();
            e.Handled = true;  //prevent from removing the selection
        }
    }
}


