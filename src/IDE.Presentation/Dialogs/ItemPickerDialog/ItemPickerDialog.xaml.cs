using IDE.Core.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using IDE.Core.Interfaces;

namespace IDE.Core.Dialogs
{

    /// <summary>
    /// Dialog window that will show a list of items, and will pick one of them
    /// </summary>
    public partial class ItemPickerDialog : ModernWindow, IItemPickerDialog
    {
        public ItemPickerDialog()
        {
            InitializeComponent();
        }



        public IList Items { get; private set; }

        public object SelectedItem { get; set; }

        public void LoadData(IList items)
        {
            Items = items;
            SelectedItem = items.Cast<object>().FirstOrDefault();
            DataContext = this;
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SelectedItem == null)
                    throw new Exception("You must select an item");

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
