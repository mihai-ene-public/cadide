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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDE.Documents.Views
{
    /// <summary>
    /// Interaction logic for ComponentDesignerView.xaml
    /// </summary>
    public partial class ComponentDesignerView : UserControl
    {
        public ComponentDesignerView()
        {
            InitializeComponent();
        }

        //void FootprintCanvas_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var canvas = sender as ZoomableCanvas;
        //        canvas.SizeChanged += (s, eo) =>
        //        {
        //            try
        //            {
        //                var cModel = (canvas.DataContext as FootprintDisplay).Canvas;
        //                cModel.Viewbox = new Rect(canvas.DesiredSize);
        //                cModel.ZoomToFit();
        //            }
        //            catch { }
        //        };

        //        var fp = canvas.DataContext as FootprintDisplay;
        //        if (fp != null)
        //            fp.Canvas.Viewbox = new Rect(canvas.RenderSize);
        //    }
        //    catch { }
        //}

        //void SymbolCanvas_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var canvas = sender as ZoomableCanvas;
        //        canvas.SizeChanged += (s, eo) =>
        //        {
        //            try
        //            {
        //                var cModel = (canvas.DataContext as GateDisplay).Canvas;
        //                cModel.Viewbox = new Rect(canvas.DesiredSize);
        //                cModel.ZoomToFit();
        //            }
        //            catch { }
        //        };
        //        (canvas.DataContext as GateDisplay).Canvas.Viewbox = new Rect(canvas.DesiredSize);
        //    }
        //    catch { }
        //}

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            IDE.Core.Utilities.ProcessStarter.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }


    }
}
