using IDE.Core.Build;
using IDE.Core.Interfaces;
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
        public BomOutputWriter()
        {
        }


        public async Task Build(IBoardDesigner board, string csvPath)
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;
            var columns = buildOptions.Bom.Columns;
            var groupColumns = buildOptions.Bom.GroupColumns;

            var bomHelper = new BomHelper();
            var list = await bomHelper.GetOutputData(board, columns, groupColumns);

            var csvWriter = new CsvWriter();
            await csvWriter.WriteCsv(list, csvPath);
        }

        
    }

}
