using System;

namespace IDE.Controls.WPF.Docking.Controls;

internal class WindowHookHandler
{
    #region Members

    private IntPtr _windowHook;
    private Win32Helper.HookProc _hookProc;
    private ReentrantFlag _insideActivateEvent = new ReentrantFlag();

    #endregion

    #region Constructors

    public WindowHookHandler()
    {
    }

    #endregion

    #region Public Methods

    public void Attach()
    {
        _hookProc = new Win32Helper.HookProc(this.HookProc);
        _windowHook = Win32Helper.SetWindowsHookEx(
            Win32Helper.HookType.WH_CBT,
            _hookProc,
            IntPtr.Zero,
            (int)Win32Helper.GetCurrentThreadId());
    }

    public void Detach()
    {
        Win32Helper.UnhookWindowsHookEx(_windowHook);
    }

    public int HookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code == Win32Helper.HCBT_SETFOCUS)
        {
            if (FocusChanged != null)
                FocusChanged(this, new FocusChangeEventArgs(wParam, lParam));
        }
        else if (code == Win32Helper.HCBT_ACTIVATE)
        {
            if (_insideActivateEvent.CanEnter)
            {
                using (_insideActivateEvent.Enter())
                {
                    //if (Activate != null)
                    //    Activate(this, new WindowActivateEventArgs(wParam));
                }
            }
        }


        return Win32Helper.CallNextHookEx(_windowHook, code, wParam, lParam);
    }

    #endregion

    #region Events

    public event EventHandler<FocusChangeEventArgs> FocusChanged;
    //public event EventHandler<WindowActivateEventArgs> Activate;

    #endregion
}
