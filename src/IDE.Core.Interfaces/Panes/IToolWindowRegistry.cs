namespace IDE.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using IDE.Core.Interfaces;

    /// <summary>
    /// Defines an interface for a class that can register
    /// and manage tool window viemodels.
    /// </summary>
    public interface IToolWindowRegistry : IService
    {
        IList<IToolWindow> Tools { get; }

        IOutput Output { get; }

        IErrorsToolWindowViewModel Errors { get; }

        void RegisterTool(IToolWindow newTool);

        void PublishTools();

        T GetTool<T>() where T : IToolWindow;
    }
}
