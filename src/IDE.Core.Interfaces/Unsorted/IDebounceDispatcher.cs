using System;

namespace IDE.Core
{
    public interface IDebounceDispatcher
    {
        void Debounce(int interval, Action<object> action, object param = null);
        void Throttle(int interval, Action<object> action, object param = null);
    }
}