using IDE.Core.Types.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;


namespace IDE.Core.Settings
{
    public class EnvironmentKeyboardSetting : BasicSetting
    {

        public List<KeyboardSetting> KeySettings { get; set; } = new List<KeyboardSetting>();

    }


    public class KeyboardSetting
    {
        //this is unique in a list
        public KeyboardOperations Operation { get; set; }

        public XModifierKeys Modifiers { get; set; } = XModifierKeys.None;

        public XKey Key { get; set; } = XKey.None;

        public KeyboardSetting Clone()
        {
            return (KeyboardSetting)MemberwiseClone();
        }
    }

    public enum KeyboardOperations
    {
        Rotate,
        MirrorX,
        MirrorY,

        Copy,
        Cut,
        Paste,
        Delete,

        Undo,
        Redo
    }
}
