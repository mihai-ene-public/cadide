using IDE.Core.Build;
using IDE.Core.Interfaces;

namespace IDE.Core.Gerber
{
    public class PickAndPlaceGerberLayerBuilder:GerberLayerBuilder
    {

        public override Task<BuildResult> Build(IBoardDesigner board, BoardGlobalLayerOutput layer, string gerberFilePath)
        {
            BuildFileAttributes(board, layer.Layer);

            PrepareBoardOutline(board);

            var result = new BuildResult { Success = true };
            result.OutputFiles.Add(gerberFilePath);

            return Task.FromResult(result);
        }
    }
}
