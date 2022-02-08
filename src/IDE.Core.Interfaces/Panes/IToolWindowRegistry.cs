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

        //SolutionExplorerViewModel SolutionToolWindow { get; }

        //PropertiesToolWindowViewModel PropertiesToolWindow { get; }

        IOutput Output { get; }

        IErrorsToolWindowViewModel Errors { get; }

        void RegisterTool(/*ToolViewModel*/IToolWindow newTool);
        void PublishTools();
    }
}
