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
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using IDE.Core.Interfaces;
using IDE.Core.ViewModels;

namespace IDE.Documents.Views
{
    /// <summary>
    /// Interaction logic for AddReferencesDialog.xaml
    /// </summary>
    public partial class AddReferencesDialog : ModernWindow, IWindow
    {
        public AddReferencesDialog()
        {
            InitializeComponent();
        }

    }
}
