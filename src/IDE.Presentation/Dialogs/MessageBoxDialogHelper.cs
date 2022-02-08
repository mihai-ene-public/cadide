using xctk = Xceed.Wpf.Toolkit;
using System.Windows;
using IDE.Core.Interfaces;

namespace IDE.Core.Utilities
{
    public class MessageBoxDialogHelper : IMessageBoxDialogHelper
    {
        public XMessageBoxResult Show(string messageBoxText)
        {
            return (XMessageBoxResult)xctk.MessageBox.Show(messageBoxText);
        }

        public XMessageBoxResult Show(string messageBoxText, string caption)
        {
            return (XMessageBoxResult)xctk.MessageBox.Show(messageBoxText, caption);
        }

        public XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button)
        {
            return (XMessageBoxResult)xctk.MessageBox.Show(messageBoxText, caption, (MessageBoxButton)button);
        }

        public XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button, XMessageBoxImage icon)
        {
            return (XMessageBoxResult)xctk.MessageBox.Show(messageBoxText, caption, (MessageBoxButton)button, MessageBoxImage.None);
        }
    }
}
