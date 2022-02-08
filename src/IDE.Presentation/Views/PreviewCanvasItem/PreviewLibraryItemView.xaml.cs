using IDE.Core;
using IDE.Core.Controls;
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

namespace IDE.Documents.Views
{
    /// <summary>
    /// Interaction logic for PreviewLibraryItemView.xaml
    /// </summary>
    public partial class PreviewLibraryItemView : UserControl
    {
        public PreviewLibraryItemView()
        {
            InitializeComponent();

            DataContextChanged += PreviewCanvasItemView_DataContextChanged;
        }

        void PreviewCanvasItemView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var c = this.FindChild<DrawingCanvas>();

            canvas = c;

            SetCanvas();
        }

        DrawingCanvas canvas;

        void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = (DrawingCanvas)sender;
        }

        //private void DesignerCanvas_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    canvas = null;
        //}

        void SetCanvas()
        {
            var preview = DataContext as PreviewLibraryItemViewModel;
            if (preview != null)// && canvas != null)
            {
                preview.ZoomToFit();
            }

        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            canvas?.RaisePreviewMouseMove(e);
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            canvas?.RaisePreviewMouseWheel(e);
        }
    }
}
