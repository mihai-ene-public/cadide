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
    private readonly IServiceProviderHelper _serviceProviderHelper;

    public ToolWindowRegistry(IServiceProviderHelper serviceProviderHelper)
    {
        _serviceProviderHelper = serviceProviderHelper;
    }

    public IList<IToolWindow> Tools
    {
        get
        {
            return tools;
        }
    }

    public void PublishTools()
    {
        var toolWindows = _serviceProviderHelper.GetServices<IToolWindow>();

        tools.AddRange(toolWindows);

    }

    public T GetTool<T>() where T : IToolWindow
    {
        return tools.OfType<T>().FirstOrDefault();
    }
}
