using IDE.Core.Build;
using IDE.Core.Interfaces;

namespace IDE.Core.PDF
{
    public class PdfBoardOutput
    {
        public PdfBoardOutput()
        {
        }

        public double DocumentWidth { get; set; } = 297.0d;
        public double DocumentHeight { get; set; } = 210.0d;

        public async Task<string> Build(IBoardDesigner board, IEnumerable<BoardLayerOutput> layers, string savePath)
        {
            var pdf = new PdfDocument();

            foreach (var layer in layers)
            {
                var page = new PdfBoardPage(board, pdf, layer, DocumentWidth, DocumentHeight);
                await page.BuildPage();
            }

            pdf.Save(savePath);

            return savePath;
        }

    }
}
