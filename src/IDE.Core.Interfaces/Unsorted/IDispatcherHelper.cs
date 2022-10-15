using System;

namespace IDE.Core.Interfaces
{
    public interface IDispatcherHelper
    {
        void RunOnDispatcher(Action action);
    }
}
