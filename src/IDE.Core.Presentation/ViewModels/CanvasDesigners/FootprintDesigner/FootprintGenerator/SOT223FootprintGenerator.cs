using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SOT223FootprintGenerator : FootprintGenerator
    {
        public SOT223FootprintGenerator(ILayeredViewModel doc) : base(doc)
        {

        }

        public override string Name => "SOT223";


        int numberPads = 4;
        /// <summary>
        /// Pin pitch
        /// </summary>
        public int NumberPads
        {
            get { return numberPads; }
            set
            {
                numberPads = value;
                OnPropertyChanged(nameof(NumberPads));
            }
        }

        public List<int> NumberPadsAvailable => new List<int> { 4 };//, 5 };

        double e = 3.5d;
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

        double e1 = 1.4;

        /// <summary>
        /// Body width
        /// </summary>
        public double E1
        {
            get { return e1; }
            set
            {
                e1 = value;
                OnPropertyChanged(nameof(E1));
            }
        }

        double d = 3.0d;
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

        double ee = 1.03d;
        /// <summary>
        /// Pin pitch
        /// </summary>
        public double EE//e
        {
            get { return ee; }
            set
            {
                ee = value;
                OnPropertyChanged(nameof(EE));
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

        double b = 0.8d;
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

        double b1 = 1.4d;
        /// <summary>
        /// Pad width in mm
        /// </summary>
        public double B1
        {
            get { return b1; }
            set
            {
                b1 = value;
                OnPropertyChanged(nameof(B1));
            }
        }

        const double pinThickness = 0.16d;

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();

                var origin = new XPoint(D, D);
                var edgeSize = 3;
                var padOffset = 0.5 * (D - (edgeSize - 1) * EE);
                var height = 0.25 * (e - e1);

                var posY = 0.5 * e1 + height;


                for (int i = 0; i < numberPads - 1; i++)
                {
                    newItems.Add(CreatePad(new XPoint(-0.5 * D + padOffset + i * EE, posY), origin, i + 1));
                }

                //pin tab
                newItems.Add(CreatePad(new XPoint(0, -posY), origin, numberPads, b1, L));

                //body
                CreateBodySilk(newItems, origin);

                return newItems;
            });
        }

        BaseCanvasItem CreatePad(XPoint position, XPoint origin, int padNumber, double width, double height)
        {
            var padPrimitive = new Smd
            {
                x = origin.X + position.X,
                y = origin.Y + position.Y,
                // rot = rot,
                Width = width,
                Height = height,
                number = (padNumber).ToString(),
            };

            var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
            (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

            return pad;
        }

        BaseCanvasItem CreatePad(XPoint position, XPoint origin, int padNumber)
        {
            return CreatePad(position, origin, padNumber, b, L);
        }

        void CreateBodySilk(List<BaseCanvasItem> newItems, XPoint origin)
        {
            var primitives = new List<LayerPrimitive>();
            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d,
                y1 = origin.Y - 0.5 * e1,
                x2 = origin.X - 0.5 * d,
                y2 = origin.Y + 0.5 * e1,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X + 0.5 * d,
                y1 = origin.Y - 0.5 * e1,
                x2 = origin.X + 0.5 * d,
                y2 = origin.Y + 0.5 * e1,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d,
                y1 = origin.Y - 0.5 * e1,
                x2 = origin.X + 0.5 * d,
                y2 = origin.Y - 0.5 * e1,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * d,
                y1 = origin.Y + 0.5 * e1,
                x2 = origin.X + 0.5 * d,
                y2 = origin.Y + 0.5 * e1,
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
