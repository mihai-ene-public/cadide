using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IDE.Controls.WPF.Windows;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views;
/// <summary>
/// Interaction logic for CheckUpdatesDialog.xaml
/// </summary>
public partial class CheckUpdatesDialog : ModernWindow, IWindow
{
    public CheckUpdatesDialog()
    {
        InitializeComponent();
    }
}
