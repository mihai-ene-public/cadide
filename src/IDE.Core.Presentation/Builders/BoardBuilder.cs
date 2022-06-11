using System.IO;
using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Builders
{
    public class BoardBuilder : IBoardBuilder
    {
        public async Task<BuildResult> Build(IBoardDesigner board)
        {
            var project = board.ProjectNode;
            var folderOutput = Path.Combine(project.GetItemFolderFullPath(), "!Output");
            var brdName = Path.GetFileNameWithoutExtension(board.FilePath);

            //var brdBuilder = new BoardOutputBuilder();
            //var buildResult = await brdBuilder.Build(board, folderOutput, brdName);

            var brdBuilder = new BoardGlobalOutputBuilder();
            var buildResult = await brdBuilder.Build(board);

            return buildResult;
        }
    }
}
