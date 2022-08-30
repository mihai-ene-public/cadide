namespace IDE.Dialogs.About
{
    using IDE.Core.Interfaces;
    using IDE.Core.Utilities;
    using IDE.Documents.Views;
    using System;
	using System.Windows;

	public partial class AboutDlg : FirstFloor.ModernUI.Windows.Controls.ModernWindow, IWindow
	{
		public AboutDlg()
		{
			InitializeComponent();
		}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var mh = new MessageBoxDialogHelper();
        //    mh.Show("Message");

        //    mh.Show("Meesage", "Caption");

        //    mh.Show("Message", "Caption", XMessageBoxButton.OKCancel);

        //    mh.Show("Yes No", "Caption", XMessageBoxButton.YesNo, XMessageBoxImage.Error);
        //    mh.Show("Message", "Caption", XMessageBoxButton.YesNoCancel, XMessageBoxImage.Warning);

        //}
    }
}
