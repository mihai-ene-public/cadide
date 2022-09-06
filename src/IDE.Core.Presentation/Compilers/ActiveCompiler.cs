using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.Compilers;

//should be able to remove errors
//should compile only if something was changed in the open file
public class ActiveCompiler : IActiveCompiler
{
    public ActiveCompiler(IFileCompiler fileCompiler)
    {
        _dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        _errorsToolWindow = ServiceProvider.GetToolWindow<IErrorsToolWindow>();
        _fileCompiler = fileCompiler;
    }


    private readonly IErrorsToolWindow _errorsToolWindow;

    private readonly IDispatcherHelper _dispatcher;

    private readonly IFileCompiler _fileCompiler;

    private bool isRunning = false;
    private bool isScheduled = false;

    public async Task Compile(IFileBaseViewModel file)
    {
        if (isRunning == false)
        {
            isRunning = true;

            if (file != null)
            {
                try
                {
                    var listErrors = new List<IErrorMessage>();

                    var result = await _fileCompiler.Compile(file);

                    if (result != null && _errorsToolWindow != null)
                    {
                        listErrors.AddRange(result.Errors);
                    }

                    _dispatcher.RunOnDispatcher(() =>
                    {
                        _errorsToolWindow?.Clear();
                        foreach (var err in listErrors)
                        {
                            _errorsToolWindow.AddErrorMessage(err);
                        }

                        if (_errorsToolWindow.Errors.Count > 0)
                        {
                            _errorsToolWindow.IsVisible = true;
                            _errorsToolWindow.IsActive = true;
                        }
                    });
                }
                finally
                {
                    isRunning = false;
                    if (isScheduled)
                        isScheduled = false;
                }
            }
        }
    }
}
