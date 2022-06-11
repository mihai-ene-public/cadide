using IDE.Documents.Views;
using System.IO;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class BoardAssemblyOutputPickAndPlaceService
    {
        public BoardAssemblyOutputPickAndPlaceService(BoardDesignerFileViewModel boardModel)
        {
            board = boardModel;
        }

        BoardDesignerFileViewModel board;

        public async Task<string> Build()
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;
            var columns = buildOptions.Assembly.PickAndPlaceColumns;

            var pickAndPlaceHelper = new AssemblyPickAndPlaceHelper();
            var list = await pickAndPlaceHelper.GetOutputData(board, columns);

            var csvPath = GetCsvFilePath();
            var csvWriter = new CsvWriter(buildOptions.Assembly.FieldSeparator);
            await csvWriter.WriteCsv(list, csvPath);

            return csvPath;
        }

        private string GetCsvFilePath()
        {
            var project = board.ProjectNode;
            if (project == null)
                return null;

            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var brdName = Path.GetFileNameWithoutExtension(board.FilePath);
            savePath = Path.Combine(savePath, $"{brdName}-PickAndPlace.csv");

            return savePath;
        }
    }

}
