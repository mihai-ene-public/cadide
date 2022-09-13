using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IDE.Controls.WPF.Windows;

public class ModernWindow : DpiAwareWindow
{
    /// <summary>
    /// Identifies the BackgroundContent dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernWindow));
    /// <summary>
    /// Identifies the IsTitleVisible dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTitleVisibleProperty = DependencyProperty.Register("IsTitleVisible", typeof(bool), typeof(ModernWindow), new PropertyMetadata(false));
    /// <summary>
    /// Identifies the LogoData dependency property.
    /// </summary>
    public static readonly DependencyProperty LogoDataProperty = DependencyProperty.Register("LogoData", typeof(Geometry), typeof(ModernWindow));
    /// <summary>
    /// Defines the ContentSource dependency property.
    /// </summary>
    public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register("ContentSource", typeof(Uri), typeof(ModernWindow));


    /// <summary>
    /// Initializes a new instance of the <see cref="ModernWindow"/> class.
    /// </summary>
    public ModernWindow()
    {
        DefaultStyleKey = typeof(ModernWindow);

        // associate window commands with this instance
        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
    }

    private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
    }

    private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode != ResizeMode.NoResize;
    }

    private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }

    private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }

    /// <summary>
    /// Gets or sets the background content of this window instance.
    /// </summary>
    public object BackgroundContent
    {
        get { return GetValue(BackgroundContentProperty); }
        set { SetValue(BackgroundContentProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window title is visible in the UI.
    /// </summary>
    public bool IsTitleVisible
    {
        get { return (bool)GetValue(IsTitleVisibleProperty); }
        set { SetValue(IsTitleVisibleProperty, value); }
    }

    /// <summary>
    /// Gets or sets the path data for the logo displayed in the title area of the window.
    /// </summary>
    public Geometry LogoData
    {
        get { return (Geometry)GetValue(LogoDataProperty); }
        set { SetValue(LogoDataProperty, value); }
    }

    /// <summary>
    /// Gets or sets the source uri of the current content.
    /// </summary>
    public Uri ContentSource
    {
        get { return (Uri)GetValue(ContentSourceProperty); }
        set { SetValue(ContentSourceProperty, value); }
    }

}

