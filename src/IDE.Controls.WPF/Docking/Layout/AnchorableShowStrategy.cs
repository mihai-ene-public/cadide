using System;

namespace IDE.Controls.WPF.Docking.Layout;

[Flags]
public enum AnchorableShowStrategy : byte
{
    Most = 0x0001,
    Left = 0x0002,
    Right = 0x0004,
    Top = 0x0010,
    Bottom = 0x0020,
}
