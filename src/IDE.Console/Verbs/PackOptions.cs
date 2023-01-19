using CommandLine;

namespace IDE.Cli.Verbs;

[Verb("pack", aliases: new[] { "p" }, HelpText = "Create package output for a solution or project")]
internal class PackOptions
{
    /// <summary>
    /// File path to a solution or a project
    /// </summary>
    [Option('f', "file", Required = true)]
    public string FilePath { get; set; }
}
