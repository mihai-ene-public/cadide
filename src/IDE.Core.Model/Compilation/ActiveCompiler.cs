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


            //timer = new DispatcherTimer(DispatcherPriority.Background);
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Tick += Timer_Tick;

            // timer.Start();

            _dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        //  IApplicationViewModel application;
        // DispatcherTimer timer;

        IErrorsToolWindowViewModel errors;

        IDispatcherHelper _dispatcher;
        //public bool IsEnabled
        //{
        //    get { return timer.IsEnabled; }
        //    set
        //    {
        //        timer.IsEnabled = value;
        //    }
        //}

        void EnsureServices()
        {
            //if (application == null)
            //    application = ServiceProvider.Resolve<IApplicationViewModel>();//ServiceProvider.GetService<IApplicationViewModel>();

            if (errors == null)
                errors = ServiceProvider.GetToolWindow<IErrorsToolWindowViewModel>();
        }

        //async void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (isRunning)
        //        return;//todo: could cancel, but schedule for next time

        //    isRunning = true;

        //    EnsureServices();

        //    if (application == null || application.Files.Count == 0)
        //        return;

        //    if (!isScheduled)
        //        timer.Stop();
        //    try
        //    {
        //        //Debug.Assert(errors != null);

        //        //clear errors
        //        errors?.Clear();

        //        foreach (var file in application.Files)
        //        {
        //            await file.Compile();

        //            if (errors != null)
        //            {
        //                foreach (var err in file.CompileErrors)
        //                {
        //                    errors.AddErrorMessage(err);
        //                }
        //            }
        //        }

        //        if (errors.Errors.Count > 0)
        //        {
        //            errors.IsVisible = true;
        //            errors.IsActive = true;
        //        }

        //    }
        //    catch { }
        //    finally
        //    {
        //        //timer.Start();
        //        isRunning = false;
        //        if (isScheduled)
        //            isScheduled = false;

        //        // timer.Start();
        //    }
        //}

        bool isRunning = false;
        bool isScheduled = false;

        //public void Run()
        //{
        //    //return;

        //    if (isRunning)
        //    {
        //        isScheduled = true;
        //        if (timer.IsEnabled)
        //            return;
        //    }
        //    else
        //        isScheduled = true;

        //    if (!timer.IsEnabled)
        //        timer.Start();
        //}

        //public void Run()
        //{
        //    if (isRunning)
        //        return;//todo: could cancel, but schedule for next time

        //    isRunning = true;

        //    EnsureServices();

        //    if (application == null || application.Files.Count == 0)
        //        return;

        //    //if (!isScheduled)
        //    //    timer.Stop();
        //    try
        //    {
        //        //  Debug.Assert(errors != null);

        //        //clear errors


        //        var listErrors = new List<IErrorMessage>();

        //        foreach (var file in application.Files)
        //        {
        //            file.Compile();

        //            if (errors != null)
        //            {
        //                listErrors.AddRange(file.CompileErrors);
        //            }
        //        }

        //        Extensions.RunOnDispatcher(() =>
        //        {
        //            errors?.Clear();
        //            foreach (var err in listErrors)
        //            {
        //                errors.AddErrorMessage(err);
        //            }

        //            if (errors.Errors.Count > 0)
        //            {
        //                errors.IsVisible = true;
        //                errors.IsActive = true;
        //            }
        //        });


        //    }
        //    catch { }
        //    finally
        //    {
        //        //timer.Start();
        //        isRunning = false;
        //        if (isScheduled)
        //            isScheduled = false;

        //        // timer.Start();
        //    }
        //}

        public async Task Compile(IFileBaseViewModel file)
        {
            if (isRunning == false)
            {
                isRunning = true;

                EnsureServices();

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
