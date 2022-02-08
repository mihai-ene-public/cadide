using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;

namespace IDE.Documents.Views
{
    public partial class BoardDesignerView : UserControl
    {
        public BoardDesignerView()
        {
            InitializeComponent();
        }

        void BoardDesignerView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var brd = DataContext as IBoardDesigner;
            if (brd != null)// && IsFocused)
            {
                if (e.Key >= Key.D1 && e.Key <= Key.D9)
                {
                    var layerNumber = (int)e.Key - (int)Key.D0;
                    brd.ChangeToCopperLayer(layerNumber);
                }
            }
        }
    }
}
