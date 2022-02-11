namespace IDE.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using IDE.Core.Errors;
    using IDE.Core.Interfaces;


    /// <summary>
    /// Class to register and manage all tool windows in one common place.
    /// </summary>
    //[Export(typeof(IToolWindowRegistry))]
    public class ToolWindowRegistry : IToolWindowRegistry
    {
        private readonly static IList<IToolWindow> tools = new ObservableCollection<IToolWindow>();

        private readonly List<IToolWindow> todoTools = new List<IToolWindow>();

        public ToolWindowRegistry()
        {
        }

        public IList<IToolWindow> Tools
        {
            get
            {
                return tools;
            }
        }

        public SolutionExplorerViewModel SolutionToolWindow
        {
            get
            {
                return tools.OfType<SolutionExplorerViewModel>().FirstOrDefault();
            }
        }

        public PropertiesToolWindowViewModel PropertiesToolWindow
        {
            get
            {
                return tools.OfType<PropertiesToolWindowViewModel>().FirstOrDefault();
            }
        }

        public IOutput Output
        {
            get
            {
                if (tools.Count > 0)
                    return tools.OfType<IOutput>().FirstOrDefault();
                else if (todoTools.Count>0)
                    return todoTools.OfType<IOutput>().FirstOrDefault();

                return null;
            }
        }

        public IErrorsToolWindowViewModel Errors
        {
            get { return tools.OfType<ErrorsToolWindowViewModel>().FirstOrDefault(); }
        }

        public LayersToolWindowViewModel Layers
        {
            get { return tools.OfType<LayersToolWindowViewModel>().FirstOrDefault(); }
        }

        public DocumentOverviewViewModel DocumentOverview
        {
            get { return tools.OfType<DocumentOverviewViewModel>().FirstOrDefault(); }
        }

        public SchematicSheetsViewModel SchematicSheets
        {
            get { return tools.OfType<SchematicSheetsViewModel>().FirstOrDefault(); }
        }


        /// <summary>
        /// Publishs all registered tool window definitions into an observable collection.
        /// (Which in turn will execute the LayoutInitializer that takes care of default positions etc).
        /// </summary>
        public void PublishTools()
        {
            tools.AddRange(todoTools);

            todoTools.Clear();
        }

        /// <summary>
        /// Register a new tool window definition for usage in this program.
        /// </summary>
        /// <param name="newTool"></param>
        public void RegisterTool(IToolWindow newTool)
        {
            try
            {
                var exists = todoTools.Any(t => t.GetType() == newTool.GetType());
                if (exists)
                    return;

                todoTools.Add(newTool);
            }
            catch (Exception exp)
            {
                throw new Exception("Tool window registration failed in ToolWindowRegistry.", exp);
            }
        }

        public T GetTool<T>() where T : IToolWindow
        {
            return tools.OfType<T>().FirstOrDefault();
        }
    }
}
