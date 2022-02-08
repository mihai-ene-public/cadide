using IDE.Core.Interfaces;

namespace IDE.Core.Utilities
{
    public static class MessageDialog
    {
        static MessageDialog()
        {
            messageBoxDialogHelper = ServiceProvider.Resolve<IMessageBoxDialogHelper>();
        }

        static IMessageBoxDialogHelper messageBoxDialogHelper;

        public static XMessageBoxResult Show(string messageBoxText)
        {
            return messageBoxDialogHelper.Show(messageBoxText);
        }

        public static XMessageBoxResult Show(string messageBoxText, string caption)
        {
            return messageBoxDialogHelper.Show(messageBoxText, caption);
        }

        public static XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button)
        {
            return messageBoxDialogHelper.Show(messageBoxText, caption, button);
        }

        public static XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button, XMessageBoxImage icon)
        {
            return messageBoxDialogHelper.Show(messageBoxText, caption, button, XMessageBoxImage.None);
        }
    }
}
