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

namespace IDE.Documents.Views
{
    /// <summary>
    /// Interaction logic for ComponentDesignerBomView.xaml
    /// </summary>
    public partial class ComponentDesignerBomView : UserControl
    {
        public ComponentDesignerBomView()
        {
            InitializeComponent();
        }

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            IDE.Core.Utilities.ProcessStarter.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
