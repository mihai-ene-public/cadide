using System;
using System.Windows;
using System.Windows.Controls;
using FontAwesome5;
using IDE.Core.Resources;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using IDE.Controls.WPF.Windows;

namespace IDE.Documents.Views;

public class MessageBoxDialog : ModernWindow
{
    private MessageBoxDialog()
    {
        InitComponents();

        Style = ResourceLocator.GetResource<Style>("IDE.Presentation",
                                                   "Themes/ModernDialogEx.xaml",
                                                   "EmptyDialog");
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Focusable = false;
        MinWidth = 100;
        MinHeight = 50;
        MaxHeight = 250;
        Width = 480;
        DataContext = this;

        AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click));
    }

    private FontAwesome messageBoxImage;
    private TextBlock messageText;
    private Button buttonYes;
    private Button buttonNo;
    private Button buttonOk;
    private Button buttonCancel;

    private void InitComponents()
    {
        var grid = new Grid()
        {
            //MinWidth = 350
        };
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var contentGrid = new Grid()
        {
            Margin = new Thickness(24, 16, 24, 22),
        };
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        messageBoxImage = new FontAwesome
        {
            Icon = EFontAwesomeIcon.None,
            FontSize = 32,
            VerticalAlignment = VerticalAlignment.Top,
            SnapsToDevicePixels = true,
        };
        contentGrid.Children.Add(messageBoxImage);

        messageText = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            FontSize = 14,
            // MaxWidth = 450,
            Margin = new Thickness(10, 10, 0, 0),
        };
        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalAlignment = VerticalAlignment.Top,
            Content = messageText
        };
        scrollViewer.SetValue(Grid.ColumnProperty, 1);
        contentGrid.Children.Add(scrollViewer);

        grid.Children.Add(contentGrid);

        //buttons
        var borderButtons = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(59, 57, 57))
        };
        borderButtons.SetValue(Grid.RowProperty, 1);
        var gridButtons = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(12)
        };
        gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        buttonYes = new Button
        {
            MinWidth = 65,
            Margin = new Thickness(6, 0, 0, 0),
            Visibility = Visibility.Collapsed,
            Content = "Yes",
        };
        buttonNo = new Button
        {
            MinWidth = 65,
            Margin = new Thickness(6, 0, 0, 0),
            Visibility = Visibility.Collapsed,
            Content = "No",
        };
        buttonOk = new Button
        {
            MinWidth = 65,
            Margin = new Thickness(6, 0, 0, 0),
            Visibility = Visibility.Collapsed,
            Content = "OK",
        };
        buttonCancel = new Button
        {
            MinWidth = 65,
            Margin = new Thickness(6, 0, 0, 0),
            Visibility = Visibility.Collapsed,
            Content = "Cancel",
        };

        gridButtons.Children.Add(buttonYes);
        gridButtons.Children.Add(buttonNo);
        gridButtons.Children.Add(buttonOk);
        gridButtons.Children.Add(buttonCancel);

        buttonYes.SetValue(Grid.ColumnProperty, 0);
        buttonNo.SetValue(Grid.ColumnProperty, 1);
        buttonOk.SetValue(Grid.ColumnProperty, 2);
        buttonCancel.SetValue(Grid.ColumnProperty, 3);

        borderButtons.Child = gridButtons;

        grid.Children.Add(borderButtons);

        Content = grid;

    }

    //we need this property because it is required in the "EmptyDialog" style
    public string WindowTitle
    {
        get => Title;
        set => Title = value;
    }

    private MessageBoxResult _dialogResult = MessageBoxResult.None;
    public MessageBoxResult MessageBoxResult
    {
        get { return _dialogResult; }
    }

    public static MessageBoxResult Show(string messageText)
    {
        return Show(messageText, string.Empty, MessageBoxButton.OK);
    }

    public static MessageBoxResult Show(string messageText, string caption)
    {
        return Show(messageText, caption, MessageBoxButton.OK, MessageBoxImage.None);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption)
    {
        return Show(owner, messageText, caption, MessageBoxButton.OK, MessageBoxImage.None);
    }

    public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button)
    {
        return Show(messageText, caption, button, MessageBoxImage.None);
    }

    public static MessageBoxResult Show(Window owner, string messageText, string caption, MessageBoxButton button)
    {
        return Show(owner, messageText, caption, button, MessageBoxImage.None);
    }

    public static MessageBoxResult Show(string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        return Show(Application.Current.MainWindow, messageText, caption, button, icon);
    }


    public static MessageBoxResult Show(Window owner, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        return ShowCore(owner, messageText, caption, button, icon);
    }

    private static MessageBoxResult ShowCore(Window owner, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
        var msgBox = new MessageBoxDialog();
        msgBox.Owner = owner;
        msgBox.Title = caption;
        msgBox.messageText.Text = messageText;

        switch (icon)
        {
            case MessageBoxImage.Information:
                msgBox.messageBoxImage.Icon = EFontAwesomeIcon.Solid_InfoCircle;
                msgBox.Foreground = new SolidColorBrush(Colors.AliceBlue);
                break;

            case MessageBoxImage.Warning:
                msgBox.messageBoxImage.Icon = EFontAwesomeIcon.Solid_ExclamationTriangle;
                msgBox.Foreground = new SolidColorBrush(Colors.Orange);
                break;

            case MessageBoxImage.Error:
                msgBox.messageBoxImage.Icon = EFontAwesomeIcon.Solid_StopCircle;
                msgBox.Foreground = new SolidColorBrush(Colors.Brown);
                break;

            case MessageBoxImage.Question:
                msgBox.messageBoxImage.Icon = EFontAwesomeIcon.Solid_QuestionCircle;
                msgBox.Foreground = new SolidColorBrush(Colors.CornflowerBlue);
                break;

            default:
                msgBox.messageBoxImage.Icon = EFontAwesomeIcon.None;
                break;
        }

        switch (button)
        {
            case MessageBoxButton.OK:
                msgBox.buttonOk.Visibility = Visibility.Visible;
                break;

            case MessageBoxButton.OKCancel:
                msgBox.buttonOk.Visibility = Visibility.Visible;
                msgBox.buttonCancel.Visibility = Visibility.Visible;
                break;
            case MessageBoxButton.YesNo:
                msgBox.buttonYes.Visibility = Visibility.Visible;
                msgBox.buttonNo.Visibility = Visibility.Visible;
                break;
            case MessageBoxButton.YesNoCancel:
                msgBox.buttonYes.Visibility = Visibility.Visible;
                msgBox.buttonNo.Visibility = Visibility.Visible;
                msgBox.buttonCancel.Visibility = Visibility.Visible;
                break;
        }


        msgBox.ShowDialog();
        return msgBox.MessageBoxResult;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = e.OriginalSource as Button;

        if (button == null)
            return;

        if (button == buttonNo)
        {
            _dialogResult = MessageBoxResult.No;
            Close();
        }
        else if (button == buttonYes)
        {
            _dialogResult = MessageBoxResult.Yes;
            Close();
        }
        else if (button == buttonCancel)
        {
            _dialogResult = MessageBoxResult.Cancel;
            Close();
        }
        else if (button == buttonOk)
        {
            _dialogResult = MessageBoxResult.OK;
            Close();
        }

        e.Handled = true;
    }

}
