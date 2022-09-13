using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Controls.WPF.Windows.Win32;
public static class ModernUIHelper
{
    /// <summary>
    /// Gets the DPI awareness of the current process.
    /// </summary>
    /// <returns>
    /// The DPI awareness of the current process
    /// </returns>
    /// <exception cref="System.ComponentModel.Win32Exception"></exception>
    public static ProcessDpiAwareness GetDpiAwareness()
    {
        ProcessDpiAwareness value;
        var result = NativeMethods.GetProcessDpiAwareness(IntPtr.Zero, out value);
        if (result != NativeMethods.S_OK)
        {
            throw new Win32Exception(result);
        }

        return value;
    }

    /// <summary>
    /// Attempts to set the DPI awareness of this process to PerMonitorDpiAware
    /// </summary>
    /// <returns>A value indicating whether the DPI awareness has been set to PerMonitorDpiAware.</returns>
    /// <remarks>
    /// <para>
    /// For this operation to succeed the host OS must be Windows 8.1 or greater, and the initial
    /// DPI awareness must be set to DpiUnaware (apply [assembly:DisableDpiAwareness] to application assembly).
    /// </para>
    /// <para>
    /// When the host OS is Windows 8 or lower, an attempt is made to set the DPI awareness to SystemDpiAware (= WPF default). This
    /// effectively revokes the [assembly:DisableDpiAwareness] attribute if set.
    /// </para>
    /// </remarks>
    public static bool TrySetPerMonitorDpiAware()
    {
        var awareness = GetDpiAwareness();

        // initial awareness must be DpiUnaware
        if (awareness == ProcessDpiAwareness.DpiUnaware)
        {
            return NativeMethods.SetProcessDpiAwareness(ProcessDpiAwareness.PerMonitorDpiAware) == NativeMethods.S_OK;
        }

        // return true if per monitor was already enabled
        return awareness == ProcessDpiAwareness.PerMonitorDpiAware;
    }
}
