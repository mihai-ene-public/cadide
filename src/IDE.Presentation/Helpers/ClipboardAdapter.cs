namespace IDE.Core.Resources
{
    using IDE.Core.Interfaces;
    using System.Windows;

    public class ClipboardAdapter : IClipboardAdapter
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
