using IDE.Core.Interfaces;

namespace IDE.Core.Designers;

public class NullDispatcherHelper : IDispatcherHelper
{
    public void RunOnDispatcher(Action action)
    {
        action();
    }
}
