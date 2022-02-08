using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class OutputAssemblyPickAndPlaceOptionsView : UserControl
    {
        public OutputAssemblyPickAndPlaceOptionsView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor pd)
            {
                e.Column.Header = string.IsNullOrEmpty(pd.DisplayName) ? pd.Name : pd.DisplayName;
            }
        }
    }
}
