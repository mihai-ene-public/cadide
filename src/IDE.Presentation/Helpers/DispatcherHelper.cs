using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IDE.Core
{
    public class DispatcherHelper : IDispatcherHelper
    {
        public void RunOnDispatcher(Action action)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
        }
    }
}
