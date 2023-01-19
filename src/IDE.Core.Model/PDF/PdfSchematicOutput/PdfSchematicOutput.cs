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
    //public class PdfSchematicOutput
    //{
    //    public double DocumentWidth { get; set; } = 297.0d;
    //    public double DocumentHeight { get; set; } = 210.0d;

    //    public List<string> OutputFiles { get; private set; } = new List<string>();

    //    public async Task Build(IFileBaseViewModel schematic, IList<ISheetDesignerItem> sheets, string outputFolder)
    //    {
    //        if (schematic == null)
    //            return;

    //        var schName = Path.GetFileNameWithoutExtension(schematic.FilePath);
    //        var savePath = Path.Combine(outputFolder, $"{schName}.pdf");

    //        var pdf = new PdfDocument();
    //        foreach (var sheet in sheets)
    //        {
    //            var page = new PdfSchematicPage(pdf, sheet, DocumentWidth, DocumentHeight);
    //            await page.BuildPage();
    //        }
    //        pdf.Save(savePath);

    //        OutputFiles.Clear();
    //        OutputFiles.Add(savePath);
    //    }

    //}
}
