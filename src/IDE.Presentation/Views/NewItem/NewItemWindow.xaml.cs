using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Serialization;
using IDE.Core.Interfaces;
using IDE.Controls.WPF.Windows;

namespace IDE.Documents.Views
{
    //this window is used for creating a new Solution with a project
    //open an existing solution template or sample solution
    //add a project to the current solution
    //add an item to the current folder or project

    //we need to review this: when window loads, load all what is needed (current implementation browses folders based on selection)
    //create a class that will handle the copy/browse of templates
    //create support for creating a new solution from a sample project saved as a template

    public partial class NewItemWindow : ModernWindow, IWindow
    {
        public NewItemWindow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = DataContext as NewItemWindowViewModel;

                if (model.SelectedTemplate == null)
                    throw new Exception("You must select a template");

                model.CreateItem();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }

        }
    }

    



   

}
