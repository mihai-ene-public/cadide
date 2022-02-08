using IDE.Core.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class BytesArrayEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var btnBrowse = new Button();
            btnBrowse.Content = "...";
            //btnBrowse.Width = 75;
            btnBrowse.Click += (s, e) =>
            {
                try
                {
                    var fileOpenDlg = new OpenFileDialog();
                    fileOpenDlg.Filter = "";
                    fileOpenDlg.Multiselect = false;
                    if (fileOpenDlg.ShowDialog() == true)
                    {
                        var bytes = File.ReadAllBytes(fileOpenDlg.FileName);
                        propertyItem.Value = bytes;
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message);
                }


            };
            return btnBrowse;
        }
    }
}
