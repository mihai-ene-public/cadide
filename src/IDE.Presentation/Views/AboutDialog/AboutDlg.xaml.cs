using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using IDE.Controls.WPF.Windows;
using IDE.Core.Interfaces;

namespace IDE.Dialogs.About;


public partial class AboutDlg : ModernWindow, IWindow
{
    public AboutDlg()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var source = PresentationSource.FromVisual(this);
        var matrix = source.CompositionTarget.TransformToDevice;

        var w = new Label();
        var dpi = VisualTreeHelper.GetDpi(w);

        MessageBox.Show($"x:{matrix.M11}, y:{matrix.M22}");
    }
}
