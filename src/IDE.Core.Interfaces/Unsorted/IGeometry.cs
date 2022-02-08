using IDE.Core.Types.Input;
using System;
using System.Windows.Input;

namespace IDE.Core.Interfaces
{
    public interface IGeometry//<T>
    {
        object Geometry { get; }
    }

    public interface IDispatcherHelper
    {
        void RunOnDispatcher(Action action);
    }

    public class ImageData
    {
        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }

        public byte[] Bits { get; set; }

        public byte[] MaskBits { get; set; }
    }
    public interface IBitmapImageHelper
    {
        ImageData GetImageData(byte[] imageBytes);
    }

    public interface ICommandFactory
    {
        ICommand CreateCommand(Action<object> execute);
        ICommand CreateCommand(Action<object> action, Predicate<object> canExecute);

        ICommand CreateUICommand(string text, string name, Type ownerType);
        ICommand CreateUICommand(string text, string name, Type ownerType, XKey key, XModifierKeys modifierKeys);
        ICommand CreateUICommand(string text, string name, Type ownerType, XKey key);
        ICommand CreateUICommand();
    }
}
