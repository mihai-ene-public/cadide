using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class ECapFootprintGenerator : FootprintGenerator
    {
        public ECapFootprintGenerator(ILayeredViewModel doc)
            : base(doc)
        {

        }

        public override string Name => "E-Cap";

        double d1 = 10.5d;

        /// <summary>
        /// Body width
        /// </summary>
        public double D1
        {
            get { return d1; }
            set
            {
                d1 = value;
                OnPropertyChanged(nameof(D1));
            }
        }

        double d = 14.0d;
        /// <summary>
        /// Length of the package. Includes the length of the pads
        /// </summary>
        public double D
        {
            get { return d; }
            set
            {
                d = value;
                OnPropertyChanged(nameof(D));
            }
        }


        double l = 4d;
        /// <summary>
        /// Length of landing pad in mm
        /// </summary>
        public double L
        {
            get { return l; }
            set
            {
                l = value;
                OnPropertyChanged(nameof(L));
            }
        }

        double b = 1.0d;
        /// <summary>
        /// Pad width in mm
        /// </summary>
        public double B
        {
            get { return b; }
            set
            {
                b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();



                var origin = new XPoint(D, D);
                var padPrimitive = new Smd
                {
                    x = origin.X - 0.5 * (D - L),
                    y = origin.Y + 0,
                    Width = L,
                    Height = b,
                    number = "1",
                };
                var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;
                newItems.Add(pad);

                //pad2

                padPrimitive = new Smd
                {
                    x = origin.X + 0.5 * (D - l),
                    y = origin.Y + 0,
                    Width = L,
                    Height = b,
                    number = "2",
                };
                pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

                newItems.Add(pad);

                //body
                CreateBodySilk(newItems, origin);



                return newItems;
            });
        }

        void CreateBodySilk(List<BaseCanvasItem> newItems, XPoint origin)
        {

            var points = new List<XPoint>();

            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, 0.5 * d1));
            points.Add(new XPoint(0.5 * d1, 0.5 * d1));
            points.Add(new XPoint(0.5 * d1, -0.5 * d1));
            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, -0.5 * d1));
            points.Add(new XPoint(-0.5 * d1, -0.5 * d1 + 0.25 * d1));
            points.Add(new XPoint(-0.5 * d1, 0.5 * d1 - 0.25 * d1));
            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, 0.5 * d1));

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

            foreach (var primitive in primitives)
            {
                var line = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                (line as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SilkscreenTopLayerId;
                newItems.Add(line);
            }
        }
    }
}
