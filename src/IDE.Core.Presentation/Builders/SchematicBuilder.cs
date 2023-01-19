using System.IO;
using System.Threading.Tasks;
using Eagle;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.PDF;

namespace IDE.Core.Presentation.Builders;

public class SchematicBuilder : ISchematicBuilder
{
    public async Task<BuildResult> Build(ISchematicDesigner schematic, string outputFolder)
    {
        var canvasModel = schematic.CanvasModel;
        //var pdfOutput = new PdfSchematicOutput()
        //{
        //    DocumentWidth = canvasModel.DocumentWidth,
        //    DocumentHeight = canvasModel.DocumentHeight
        //};

        //await pdfOutput.Build(schematic, schematic.Sheets, outputFolder);

        var schName = Path.GetFileNameWithoutExtension(schematic.FilePath);
        var savePath = Path.Combine(outputFolder, $"{schName}.pdf");

        var pdf = new PdfDocument();
        foreach (var sheet in schematic.Sheets)
        {
            var page = new PdfSchematicPage(pdf, sheet, canvasModel.DocumentWidth, canvasModel.DocumentHeight);
            await page.BuildPage();
        }
        pdf.Save(savePath);


        return new BuildResult
        {
            Success = true,
            OutputFiles = new[] { savePath }.ToList()
        };
    }
}
