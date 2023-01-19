using System.IO;
using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Builders
{
    public class BoardBuilder : IBoardBuilder
    {
        public async Task<BuildResult> Build(IBoardDesigner board, string folderOutput)
        {
            var brdBuilder = new BoardGlobalOutputBuilder();
            var buildResult = await brdBuilder.Build(board, folderOutput);

            return buildResult;
        }
    }
}
