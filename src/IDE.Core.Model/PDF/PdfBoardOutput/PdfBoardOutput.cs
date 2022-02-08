using IDE.Core.Build;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using OxyPlot;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Media;

namespace IDE.Core.PDF
{
    public class PdfBoardOutput
    {
        public PdfBoardOutput()
        {

        }


        public double DocumentWidth { get; set; } = 297.0d;
        public double DocumentHeight { get; set; } = 210.0d;
        //public List<string> OutputFiles { get; private set; } = new List<string>();

        public async Task<string> Build(IBoardDesigner board, IEnumerable<BoardLayerOutput> layers, string savePath)
        {
            var f = board as IFileBaseViewModel;
            if (f == null)
                return null;
            var project = f.ProjectNode;
            if (project == null)
                return null;
            //var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            //Directory.CreateDirectory(savePath);
            //var schName = Path.GetFileNameWithoutExtension(f.FilePath);
            //savePath = Path.Combine(savePath, $"{schName}{GetExtension("pdf")}");

            var pdf = new PdfDocument();

            foreach (var layer in layers)
            {
                var page = new PdfBoardPage(board, pdf, layer, DocumentWidth, DocumentHeight);
                await page.BuildPage();
            }

            pdf.Save(savePath);


            return savePath;
        }

        string GetExtension(string extension)
        {
            if (extension.StartsWith("."))
                return extension;
            return "." + extension.ToUpper();
        }
    }
}
