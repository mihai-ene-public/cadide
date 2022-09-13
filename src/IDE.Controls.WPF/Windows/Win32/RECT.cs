using System.Runtime.InteropServices;

namespace IDE.Controls.WPF.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int left, top, right, bottom;
}
