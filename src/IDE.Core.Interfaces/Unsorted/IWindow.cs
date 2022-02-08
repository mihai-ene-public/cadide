using System.Collections;
using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    public interface IWindow
    {

        double Top { get; set; }
        double Left { get; set; }
        double Width { get; set; }
        double Height { get; set; }

        object DataContext { get; set; }

        string Title { get; set; }

        bool? ShowDialog();

    }

    public interface IWindowWithCommands
    {
        void AddCommanBindings(IList<CommandBindingData> bindings);
    }

    public interface IResourceLocator
    {
        T FindResource<T>(string assemblyName, string resourceFilename, string name) where T : class;

        void SwitchToSelectedTheme();
    }

    public interface IClipboardAdapter
    {
        void SetText(string text);
    }

    public interface IModelImporter
    {
        string[] GetSupportedFileFormats();
        void Import(string filePath, IDrawingViewModel canvasModel);
    }
}
