using System.Collections.Generic;

namespace IDE.Core.PDF
{
    internal class PdfFigure : PdfPrimitive
    {

        public List<PdfPrimitive> FigureItems { get; set; } = new List<PdfPrimitive>();

        public override void WriteTo(IPdfDocument pdfDoc)
        {
            foreach (var f in FigureItems)
            {
                f.WriteTo(pdfDoc);

                //pdfDoc.EndPath();
            }
        }
    }
}