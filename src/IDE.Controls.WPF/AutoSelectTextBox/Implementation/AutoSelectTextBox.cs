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
public class AutoSelectTextBox : TextBox
{
    public AutoSelectTextBox()
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
                                                 typeof(AutoSelectTextBox),
                                                 new UIPropertyMetadata(true));


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


