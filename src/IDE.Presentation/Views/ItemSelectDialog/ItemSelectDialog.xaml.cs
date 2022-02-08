using FirstFloor.ModernUI.Windows.Controls;
using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IDE.Documents.Views
{

    //this window is used for browsing for an existing symbol or footprint in local project and references
    //in future could be others: models, etc
    public partial class ItemSelectDialog : ModernWindow, IWindow
    {
        public ItemSelectDialog()
        {
            InitializeComponent();

            //Loaded += Window_Loaded;

            //DataContext = model;
        }

        //ItemSelectDialogViewModel model = new ItemSelectDialogViewModel();

        //public TemplateType TemplateType
        //{
        //    get
        //    {
        //        return model.TemplateType;
        //    }
        //    set
        //    {
        //        model.TemplateType = value;
        //    }
        //}

        //public SolutionProjectNodeModel ProjectModel
        //{
        //    get
        //    {
        //        return model.ProjectModel;
        //    }
        //    set
        //    {
        //        model.ProjectModel = value;
        //    }
        //}

        //public ItemDisplay SelectedItem
        //{
        //    get
        //    {
        //        return model.SelectedItem;
        //    }
        //    set
        //    {
        //        model.SelectedItem = value;
        //    }
        //}

        //void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    model.LoadItems();
        //}

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = DataContext as ItemSelectDialogViewModel;
                if (model.SelectedItem == null)
                    throw new Exception("You must select an item");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }
        }



        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

            var model = DataContext as ItemSelectDialogViewModel;
            model.SelectedItem = tree.SelectedItem as ItemDisplay;
        }

        //void LibraryItemCanvas_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var canvas = sender as ZoomableCanvas;
        //    var libItem = canvas.DataContext as IDrawingViewModelItem;
        //    if (libItem != null)
        //        libItem.Canvas.Canvas = canvas;
        //    //var compItem = canvas.DataContext as ComponentItemDisplay;
        //    //if (compItem != null)
        //    //{
        //    //    compItem.Symbol.Canvas.Viewbox = new Rect(canvas.DesiredSize);
        //    //    compItem.Footprint.Canvas.Viewbox = new Rect(canvas.DesiredSize);
        //    //}

        //}

        void LibraryItemCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var canvas = sender as ZoomableCanvas;
            ////(canvas.DataContext as LibraryItemDisplay).Canvas.Viewbox = new Rect(canvas.DesiredSize);
            ////var cModel = (canvas.DataContext as LibraryItemDisplay).Canvas;
            ////cModel.ZoomToFit();
            //var libItem = canvas.DataContext as LibraryItemDisplay;
            //if (libItem != null)
            //{
            //    libItem.Canvas.Viewbox = new Rect(canvas.DesiredSize);
            //    libItem.Canvas.ZoomToFit();
            //}

            //var compItem = canvas.DataContext as ComponentItemDisplay;
            //if (compItem != null)
            //{
            //    compItem.Symbol.Canvas.Viewbox = new Rect(canvas.DesiredSize);
            //    compItem.Footprint.Canvas.Viewbox = new Rect(canvas.DesiredSize);

            //    compItem.Symbol.Canvas.ZoomToFit();
            //    compItem.Footprint.Canvas.ZoomToFit();
            //}
        }
    }

   


}
