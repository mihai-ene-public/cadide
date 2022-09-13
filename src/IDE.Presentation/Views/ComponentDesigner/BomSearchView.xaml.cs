using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using IDE.Core.Utilities;
using IDE.Controls.WPF.Windows;

namespace IDE.Documents.Views
{
    /// <summary>
    /// Interaction logic for BomSearchView.xaml
    /// </summary>
    public partial class BomSearchView : ModernWindow, IWindow
    {
        public BomSearchView()
        {
            InitializeComponent();

            //DataContext = model;
        }

        //BomSearchViewModel model = new BomSearchViewModel();

        //public BomSearchViewModel Model { get { return model; } }

        void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            ProcessStarter.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
