using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Core.Presentation.Compilers
{
    public abstract class AbstractCompiler
    {
        private IErrorMessage CreateMessage(MessageSeverity severity, string message, string projectName, IFileBaseViewModel file, XRect? location = null)
        {
            return new ErrorMessage
            {
                Severity = severity,
                Description = message,
                File = file.FileName,
                Project = projectName,
                Location = new CanvasLocation
                {
                    File = file,
                    Location = location
                }
            };
        }

        protected IErrorMessage BuildErrorMessage(string message, string projectName, IFileBaseViewModel file, XRect? location = null)
        {
            var em = CreateMessage(MessageSeverity.Error, message, projectName, file, location);
            return em;
        }

        protected IErrorMessage BuildWarningMessage(string message, string projectName, IFileBaseViewModel file, XRect? location = null)
        {
            var em = CreateMessage(MessageSeverity.Warning, message, projectName, file, location);
            return em;
        }
    }
}
