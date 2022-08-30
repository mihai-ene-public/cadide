using System.Windows;
using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Utilities;

public class MessageBoxDialogHelper : IMessageBoxDialogHelper
{
    public XMessageBoxResult Show(string messageBoxText)
    {
        return (XMessageBoxResult)MessageBoxDialog.Show(messageBoxText);
    }

    public XMessageBoxResult Show(string messageBoxText, string caption)
    {
        return (XMessageBoxResult)MessageBoxDialog.Show(messageBoxText, caption);
    }

    public XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button)
    {
        return (XMessageBoxResult)MessageBoxDialog.Show(messageBoxText, caption, (MessageBoxButton)button);
    }

    public XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button, XMessageBoxImage icon)
    {
        return (XMessageBoxResult)MessageBoxDialog.Show(messageBoxText, caption, (MessageBoxButton)button, (MessageBoxImage)icon);
    }
}
