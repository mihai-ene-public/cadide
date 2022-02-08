using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class CrystalSMDFootprintGenerator : FootprintGenerator
    {
        public CrystalSMDFootprintGenerator(ILayeredViewModel doc)
            : base(doc)
        {

        }

        public override string Name => "Crystal SMD";

        double d1 = 11.7d;

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

        double d = 13.2d;
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

        double e = 5d;
        /// <summary>
        /// Pin pitch
        /// </summary>
        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
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

        double b = 0.63d;
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
            var primitives = new List<LayerPrimitive>();
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


            foreach (var primitive in primitives)
            {
                var line = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                (line as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SilkscreenTopLayerId;
                newItems.Add(line);
            }
        }
    }
}
