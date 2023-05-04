using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Designers;
using IDE.Core.Presentation.Solution;
using System.IO;
using System.Text.Json.Nodes;
using IDE.Core.Presentation.Messages;

namespace IDE.Core.Presentation.Compilers;

public class SolutionCompiler : ISolutionCompiler
{
    public SolutionCompiler(IFileCompiler fileCompiler, ISolutionRepository solutionRepository)
    {
        _fileCompiler = fileCompiler;
        _solutionRepository = solutionRepository;
    }

    private readonly IFileCompiler _fileCompiler;
    private readonly ISolutionRepository _solutionRepository;

    public async Task<bool> CompileSolution(string solutionFilePath)
    {
        var solutionProjectFiles = _solutionRepository.GetProjectsFromSolution(solutionFilePath);

        var compileResults = new List<bool>();
        foreach (var projectFile in solutionProjectFiles)
        {
            var res = await CompileProject(projectFile);
            compileResults.Add(res);
        }

        return !compileResults.Any(c => c == false);
    }

    public async Task<bool> CompileProject(string projectFilePath)
    {
        var projectFolder = Path.GetDirectoryName(projectFilePath);
        var projectName = Path.GetFileNameWithoutExtension(projectFilePath);

        var compileResults = new List<CompilerResult>();

        SendOutputMessage($"Compiling project {projectName}");

        //there are some document editors that we cannot open at the current stage, so only some extensions supported
        var projectItemFiles = FileSystemHelper.GetFilesWithExtension(projectFolder,
                                                                    new[] { ".board",
                                                                        ".schematic",
                                                                        ".symbol",
                                                                        ".footprint",
                                                                        ".component"
                                                                    });

        foreach (var projectItemFilePath in projectItemFiles)
        {
            try
            {
                var file = await _solutionRepository.OpenDocumentAsync(projectItemFilePath);

                var result = await _fileCompiler.Compile(file);

                if (result != null)
                {
                    compileResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                SendOutputMessage($"Error: {ex.Message}");
                SendErrorMessage(ex.Message, Path.GetFileNameWithoutExtension(projectItemFilePath), projectName);
            }
        }

        //collect errors (we do it in a separate loop so that the prev loop be executed in Parallel)
        foreach (var file in compileResults)
        {
            foreach (var err in file.Errors)
            {
                SendOutputMessage($"Error: {err.Description}");
                SendErrorMessage(err);
            }
        }

        var hasErrors = compileResults.Any(f => f.Success == false);
        var resultMessage = hasErrors ? "with errors" : "successfully";

        SendOutputMessage($"Project {projectName} compiled {resultMessage}");

        return !hasErrors;
    }

    private void SendOutputMessage(string message)
    {
        Messenger.Send(message);
    }

    private void SendErrorMessage(IErrorMessage errorMessage)
    {
        Messenger.Send(errorMessage);
    }
    private void SendErrorMessage(string message, string fileName, string projectName)
    {
        var errorMessage = new ErrorMessage
        {
            Severity = MessageSeverity.Error,
            Description = message,
            File = fileName,
            Project = projectName
        };
        Messenger.Send(errorMessage);
    }
}
