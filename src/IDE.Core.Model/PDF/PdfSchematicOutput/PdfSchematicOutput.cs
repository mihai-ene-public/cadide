using IDE.Core.Designers;
using IDE.Core.Interfaces;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Media;

namespace IDE.Core.PDF
{
    public class PdfSchematicOutput
    {
        public double DocumentWidth { get; set; } = 297.0d;
        public double DocumentHeight { get; set; } = 210.0d;

        public List<string> OutputFiles { get; private set; } = new List<string>();

        public async Task Build(IFileBaseViewModel schematic, IList<ISheetDesignerItem> sheets)
        {
            var f = schematic as IFileBaseViewModel;
            if (f == null)
                return;
            var project = f.ProjectNode;
            if (project == null)
                return;
            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var schName = Path.GetFileNameWithoutExtension(f.FilePath);
            savePath = Path.Combine(savePath, $"{schName}{GetExtension("pdf")}");

            var pdf = new PdfDocument();
            foreach (var sheet in sheets)
            {
                var page = new PdfSchematicPage(pdf, sheet, DocumentWidth, DocumentHeight);
                await page.BuildPage();
            }
            pdf.Save(savePath);

            OutputFiles.Clear();
            OutputFiles.Add(savePath);
        }

        string GetExtension(string extension)
        {
            if (extension.StartsWith("."))
                return extension;
            return "." + extension.ToUpper();
        }
    }
}
