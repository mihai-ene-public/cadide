using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Build
{
    public class BoardAssemblyGlobalOutputPickAndPlaceService
    {
        public async Task Build(IBoardDesigner board, string savePath)
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;
            var columns = buildOptions.Assembly.PickAndPlaceColumns;

            var pickAndPlaceHelper = new AssemblyPickAndPlaceHelper();
            var list = await pickAndPlaceHelper.GetOutputData(board, columns);

            var csvWriter = new CsvWriter(buildOptions.Assembly.FieldSeparator);
            await csvWriter.WriteCsv(list, savePath);

        }

       
    }

}
