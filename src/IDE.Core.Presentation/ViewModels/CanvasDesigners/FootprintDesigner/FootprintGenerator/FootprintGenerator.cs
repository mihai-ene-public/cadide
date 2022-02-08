using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public abstract class FootprintGenerator : BaseViewModel
    {

        public FootprintGenerator(ILayeredViewModel document)
        {
            layeredDocument = document;
        }

        protected ILayeredViewModel layeredDocument;

        public abstract string Name { get; }

        protected void swap(ref double a, ref double b)
        {
            var c = a;
            a = b;
            b = c;
        }

        protected List<LayerPrimitive> GetLinesFromPoints(IList<XPoint>points, XPoint origin)
        {
            var primitives = new List<LayerPrimitive>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];

                primitives.Add(new LineBoard
                {
                    x1 = origin.X + start.X,
                    y1 = origin.Y + start.Y,
                    x2 = origin.X + end.X,
                    y2 = origin.Y + end.Y,
                });
            }

            return primitives;
        }

        public abstract Task<List<BaseCanvasItem>> GenerateFootprint();
    }
}
