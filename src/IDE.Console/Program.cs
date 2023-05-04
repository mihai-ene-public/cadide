using CommandLine;
using CommandLine.Text;
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Common.Extensions;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Builders;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Presentation.Messages;

namespace IDE.Cli;

internal class Program
{
    async static Task<int> Main(string[] args)
    {
        var container = Startup.BuildServices();

        var obj = new object();

        Messenger.Register<object, string>(obj, (vm, message) => Console.WriteLine(message));


        var res = await Parser.Default.ParseArguments<CompileOptions, BuildOptions, PackOptions>(args)
            .MapResult(
          (CompileOptions opts) => Compile(opts, container),
          (BuildOptions opts) => Build(opts, container),
          (PackOptions opts) => Pack(opts, container),
          errs => Task.FromResult(0));

        return res;
    }



    public static async Task<int> Compile(CompileOptions opt, IServiceProvider serviceProvider)
    {
        try
        {
            var compiler = serviceProvider.GetService<ISolutionCompiler>();
            var fileExtension = Path.GetExtension(opt.FilePath);
            if (fileExtension.StartsWith("."))
                fileExtension = fileExtension.Substring(1);

            switch (fileExtension)
            {
                case "solution":
                    var res = await compiler.CompileSolution(opt.FilePath);
                    return res ? 0 : 1;

                case "project":
                    var resp = await compiler.CompileProject(opt.FilePath);
                    return resp ? 0 : 1;

                default:
                    throw new Exception("This file type is not supported for compiling. Only solution file types allowed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }

    }

    public static async Task<int> Build(BuildOptions opt, IServiceProvider serviceProvider)
    {
        try
        {
            var compiler = serviceProvider.GetService<ISolutionBuilder>();
            var fileExtension = Path.GetExtension(opt.FilePath);
            if (fileExtension.StartsWith("."))
                fileExtension = fileExtension.Substring(1);

            switch (fileExtension)
            {
                case "solution":
                    await compiler.BuildSolution(opt.FilePath);
                    break;

                case "project":
                    await compiler.BuildProject(opt.FilePath);
                    break;

                default:
                    throw new Exception("This file type is not supported for building. Only solution file types allowed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }

        return 0;
    }

    private static async Task<int> Pack(PackOptions opt, IServiceProvider serviceProvider)
    {
        try
        {
            var compiler = serviceProvider.GetService<ISolutionPackageBuilder>();
            var fileExtension = Path.GetExtension(opt.FilePath);
            if (fileExtension.StartsWith("."))
                fileExtension = fileExtension.Substring(1);

            switch (fileExtension)
            {
                case "solution":
                    await compiler.BuildSolution(opt.FilePath);
                    break;

                case "project":
                    await compiler.BuildProject(opt.FilePath);
                    break;

                default:
                    throw new Exception("This file type is not supported for building. Only solution file types allowed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }

        return 0;
    }
}
