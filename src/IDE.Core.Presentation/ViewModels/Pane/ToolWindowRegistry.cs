using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Infrastructure;

namespace IDE.Core.ViewModels;


/// <summary>
/// Class to register and manage all tool windows in one common place.
/// </summary>
public class ToolWindowRegistry : IToolWindowRegistry
{
    private readonly static IList<IToolWindow> tools = new ObservableCollection<IToolWindow>();

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
    public void RegisterTool(IToolWindow newTool)
    {
        var exists = tools.Any(t => t.GetType() == newTool.GetType());
        if (exists)
            return;

        tools.Add(newTool);
    }

    public T GetTool<T>() where T : IToolWindow
    {
        return tools.OfType<T>().FirstOrDefault();
    }
}
