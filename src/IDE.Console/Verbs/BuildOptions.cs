using CommandLine;

namespace IDE.Cli.Verbs;

[Verb("build", aliases: new[] { "b" }, HelpText = "Build: create output for a solution or project")]
internal class BuildOptions
{
    /// <summary>
    /// File path to a solution or a project
    /// </summary>
    [Option('f', "file", Required = true)]
    public string FilePath { get; set; }
}
