using IDE.Core.Build;
using IDE.Core.Interfaces;

namespace IDE.Core.PDF
{
    public class PdfBoardGlobalOutput
    {
        public double DocumentWidth { get; set; } = 297.0d;
        public double DocumentHeight { get; set; } = 210.0d;

        public async Task<BuildResult> Build(IBoardDesigner board, IEnumerable<BoardGlobalLayerOutput> layers, string savePath)
        {
            var pdf = new PdfDocument();

            foreach (var layer in layers)
            {
                var page = new PdfBoardPageBuilder();
                await page.Build(board, pdf, layer, DocumentWidth, DocumentHeight);
            }

            pdf.Save(savePath);

            var result = new BuildResult { Success = true };
            result.OutputFiles.Add(savePath);

            return result;
        }
    }
}
