using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IDialogViewModel
    {
        bool? ShowDialog(object args = null);
    }

    public interface IMessageBoxDialogHelper
    {
        XMessageBoxResult Show(string messageBoxText);

        XMessageBoxResult Show(string messageBoxText, string caption);

        XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button);

        XMessageBoxResult Show(string messageBoxText, string caption, XMessageBoxButton button, XMessageBoxImage icon);
    }
}
