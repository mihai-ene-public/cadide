using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.PDF
{
    public class PdfPrimitive
    {
        public XColor Color { get; set; } = XColor.FromRgb(0, 0, 0);

        public virtual void WriteTo(IPdfDocument pdfDoc) { }
    }

    public interface IFilledPdfPrimitive
    {
        XColor FillColor { get; set; }
    }
}
