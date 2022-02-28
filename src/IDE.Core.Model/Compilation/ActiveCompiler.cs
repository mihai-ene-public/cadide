using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Compilation
{
    //should be able to remove errors
    //should compile only if something was changed in the open file
    public class ActiveCompiler : IActiveCompiler
    {
        public ActiveCompiler()
        {
            _dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            errors = ServiceProvider.GetToolWindow<IErrorsToolWindow>();
        }


        IErrorsToolWindow errors;

        IDispatcherHelper _dispatcher;


        bool isRunning = false;
        bool isScheduled = false;

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

                        await file.Compile();

                        if (errors != null)
                        {
                            listErrors.AddRange(file.CompileErrors);
                        }

                        _dispatcher.RunOnDispatcher(() =>
                        {
                            errors?.Clear();
                            foreach (var err in listErrors)
                            {
                                errors.AddErrorMessage(err);
                            }

                            if (errors.Errors.Count > 0)
                            {
                                errors.IsVisible = true;
                                errors.IsActive = true;
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
}
