using IDE.Controls.WPF.Windows;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views;

public partial class SettingsDialog : ModernWindow, IWindow
{
    /// <summary>
    /// Class constructor
    /// </summary>
    public SettingsDialog()
    {
        InitializeComponent();
    }

}
