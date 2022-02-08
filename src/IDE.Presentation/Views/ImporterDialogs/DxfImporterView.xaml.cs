using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using IDE.Core.Interfaces;


namespace IDE.Presentation.Views.ImporterDialogs
{
    public partial class DxfImporterView : ModernWindow, IWindow
    {
        public DxfImporterView()
        {
            InitializeComponent();
        }
    }
}
