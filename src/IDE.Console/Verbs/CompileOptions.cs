using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace IDE.Cli.Verbs;

[Verb("compile", aliases: new[] { "c" }, HelpText = "Compile: check for errors in a solution or project")]
internal class CompileOptions
{
    /// <summary>
    /// File path to a solution or a project
    /// </summary>
    [Option('f', "file", Required = true)]
    public string FilePath { get; set; }
}
