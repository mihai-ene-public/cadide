using IDE.Core.Commands;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace IDE.Core.Errors
{
    public class ErrorsToolWindowViewModel : ToolViewModel, IErrorsToolWindow
    {
        public ErrorsToolWindowViewModel()
            : base("Errors")
        {
        }

        ObservableCollection<IErrorMessage> errors = new ObservableCollection<IErrorMessage>();

        public ObservableCollection<IErrorMessage> Errors { get { return errors; } }

        ICommand navigateToErrorCommand;

        public ICommand NavigateToErrorCommand
        {
            get
            {
                if (navigateToErrorCommand == null)
                {
                    navigateToErrorCommand = CreateCommand(p =>
                      {
                          //todo: need to rewrite this
                          //var errMessage = p as IErrorMessage;
                          //if (errMessage != null)
                          //{
                          //    if (errMessage.Location != null && errMessage.Location.File != null)
                          //    {
                          //        var applicationModel = ServiceProvider.Resolve<IApplicationViewModel>();
                          //        var item = errMessage.Location.File.Item as SolutionExplorerNodeModel;

                          //        var solutionExplorer = ApplicationServices.ToolRegistry.Tools.OfType<SolutionExplorerViewModel>().FirstOrDefault();
                          //        if (solutionExplorer != null && item != null)
                          //        {
                          //            var filePath = item.GetItemFullPath();
                          //            item = solutionExplorer.FindNodeByFilePath(filePath);
                          //        }

                          //        if (item != null)
                          //        {
                          //            var filePath = item.GetItemFullPath();
                          //            var file = await applicationModel.Open(item, filePath);

                          //            if (errMessage.Location is CanvasLocation canvasLocation && canvasLocation.Location != null)
                          //            {
                          //                var canvasFile = file as CanvasDesignerFileViewModel;
                          //                if (canvasFile != null && canvasFile.CanvasModel != null)
                          //                {
                          //                    canvasFile.CanvasModel.ZoomToRectangle(canvasLocation.Location.Value);
                          //                }
                          //            }
                          //        }
                          //    }
                          //}
                      });
                }

                return navigateToErrorCommand;
            }
        }

        public void Clear()
        {
            Errors.Clear();
        }

        public void AddError(string message, string fileName, string projectName)
        {
            var em = new ErrorMessage
            {
                Severity = MessageSeverity.Error,
                Description = message,
                File = fileName,
                Project = projectName
            };
            Errors.Add(em);
        }
        public void AddErrorMessage(IErrorMessage message)
        {
            Errors.Add(message);
        }

        public void AddWarning(string message, string fileName, string projectName)
        {
            var em = new ErrorMessage
            {
                Severity = MessageSeverity.Warning,
                Description = message,
                File = fileName,
                Project = projectName
            };
            Errors.Add(em);
        }


        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Bottom;
            }
        }

    }
}
