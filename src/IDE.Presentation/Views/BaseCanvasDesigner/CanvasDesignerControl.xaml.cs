using IDE.Core.Controls;
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
    public partial class CanvasDesignerControl : UserControl
    {
        public CanvasDesignerControl()
        {
            InitializeComponent();
        }

        DrawingCanvas canvas;

        void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = (DrawingCanvas)sender;
        }

        //TODO: create a behavior for these 2 handlers below

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            canvas.RaisePreviewMouseMove(e);
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            canvas.RaisePreviewMouseWheel(e);
            e.Handled = true;
        }
       
    }
}
