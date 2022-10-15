using System.Threading.Tasks;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.PDF;

namespace IDE.Core.Presentation.Builders;

public class SchematicBuilder : ISchematicBuilder
{
    public async Task<BuildResult> Build(ISchematicDesigner schematic)
    {
        var canvasModel = schematic.CanvasModel;
        var pdfOutput = new PdfSchematicOutput()
        {
            DocumentWidth = canvasModel.DocumentWidth,
            DocumentHeight = canvasModel.DocumentHeight
        };

        await pdfOutput.Build(schematic, schematic.Sheets);

        return new BuildResult
        {
            Success = true,
            OutputFiles = pdfOutput.OutputFiles
        };
    }
}
