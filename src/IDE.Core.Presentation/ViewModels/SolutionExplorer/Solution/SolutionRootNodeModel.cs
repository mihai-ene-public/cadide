using IDE.Core.Compilation;
using IDE.Core.Storage;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Common;

namespace IDE.Core.ViewModels
{

    /// <summary>
    /// root solution node. It contains SolutionDocument as Document
    /// </summary>
    public class SolutionRootNodeModel : SolutionExplorerNodeModel, ISolutionRootNodeModel
    {

        public ISolutionDocument Solution { get { return Document as SolutionDocument; } }

        protected override string GetNameInternal()
        {
            return Path.GetFileNameWithoutExtension(SolutionManager.SolutionFilePath);//SolutionDocument.FilePath);
        }

        public override void Load(string filePath)
        {
            Children.Clear();

            //load solution file from disk
            var solution = SolutionManager.LoadSolution(filePath);//SolutionDocument.Load(filePath);
            Document = solution;

            if (solution.Children != null)
            {
                foreach (var child in solution.Children.AsParallel())
                {

                    var nodeModel = child.CreateSolutionExplorerNodeModel();
                    AddChild(nodeModel);
                }
            }
        }

        public async override Task Compile()
        {
            var compiler = new Compiler();
            await compiler.CompileSolution(this);
        }

        public async override Task Build()
        {
            var compiler = new Compiler();
            await compiler.BuildSolution(this);
        }
    }
}
