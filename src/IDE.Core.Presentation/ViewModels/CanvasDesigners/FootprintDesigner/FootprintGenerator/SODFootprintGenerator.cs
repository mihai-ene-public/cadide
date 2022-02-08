using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SODFootprintGenerator : FootprintGenerator
    {
        public SODFootprintGenerator(ILayeredViewModel doc) : base(doc)
        {

        }

        public override string Name => "SOD";

        double e = 1.8d;

        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
            }
        }

        double d1 = 3.5;

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

        double d = 5.2d;
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



        double l = 0.6d;
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

        double b = 0.6d;
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

                var posX = 0.5 * d - 0.5 * l;

                newItems.Add(CreatePad(new XPoint(-posX, 0), origin, 1));
                newItems.Add(CreatePad(new XPoint(posX, 0), origin, 2));

                //body
                CreateBodySilk(newItems, origin);

                return newItems;
            });
        }

        BaseCanvasItem CreatePad(XPoint position, XPoint origin, int padNumber)
        {
            var padPrimitive = new Smd
            {
                x = origin.X + position.X,
                y = origin.Y + position.Y,
                // rot = rot,
                Width = L,
                Height = b,
                number = (padNumber).ToString(),
            };


            var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
            (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

            return pad;
        }

        void CreateBodySilk(List<BaseCanvasItem> newItems, XPoint origin)
        {
            var primitives = new List<LayerPrimitive>();
            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d1,
                y1 = origin.Y - 0.5 * e,
                x2 = origin.X - 0.5 * d1,
                y2 = origin.Y + 0.5 * e,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X + 0.5 * d1,
                y1 = origin.Y - 0.5 * e,
                x2 = origin.X + 0.5 * d1,
                y2 = origin.Y + 0.5 * e,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d1,
                y1 = origin.Y - 0.5 * e,
                x2 = origin.X + 0.5 * d1,
                y2 = origin.Y - 0.5 * e,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d1,
                y1 = origin.Y + 0.5 * e,
                x2 = origin.X + 0.5 * d1,
                y2 = origin.Y + 0.5 * e,
            });


            foreach (var primitive in primitives)
            {
                var line = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                (line as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SilkscreenTopLayerId;
                newItems.Add(line);
            }
        }

    }
}
