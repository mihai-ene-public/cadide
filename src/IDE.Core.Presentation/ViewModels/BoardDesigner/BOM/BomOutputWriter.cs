using IDE.Core.Build;
using IDE.Documents.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.BOM
{
    public class BomOutputWriter
    {
        public BomOutputWriter(BoardDesignerFileViewModel boardModel)
        {
            board = boardModel;
        }

        BoardDesignerFileViewModel board;

        public List<string> OutputFiles { get; private set; } = new List<string>();

        public async Task Build()
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;
            var columns = buildOptions.Bom.Columns;
            var groupColumns = buildOptions.Bom.GroupColumns;

            var bomHelper = new BomHelper();
            var list = await bomHelper.GetOutputData(board, columns, groupColumns);

            var csvPath = GetCsvFilePath();
            var csvWriter = new CsvWriter();
            await csvWriter.WriteCsv(list, csvPath);
            
            OutputFiles.Clear();
            OutputFiles.Add(csvPath);
        }

        private string GetCsvFilePath()
        {
            var project = board.ProjectNode;
            if (project == null)
                return null;

            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var brdName = Path.GetFileNameWithoutExtension(board.FilePath);
            savePath = Path.Combine(savePath, $"{brdName}-BOM.csv");

            return savePath;
        }
    }

}
