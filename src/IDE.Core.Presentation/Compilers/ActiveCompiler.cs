using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Messages;
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
        _fileCompiler = fileCompiler;
    }

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
                    Messenger.Send(new ClearErrorsMessage());

                    var result = await _fileCompiler.Compile(file);

                    if (result != null)
                    {
                        if (result.Errors.Count > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                Messenger.Send(error);
                            }

                            Messenger.Send(new ActivateErrorsToolWindow
                            {
                                IsActive = true,
                                IsVisible = true,
                            });
                        }

                    }

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
