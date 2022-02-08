using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class PinHeaderStraightFootprintGenerator : FootprintGenerator
    {
        public PinHeaderStraightFootprintGenerator(ILayeredViewModel doc)
                   : base(doc)
        {

        }


        public override string Name => "Pin Header - Straight";

        int numberRows = 1;

        public int NumberRows
        {
            get { return numberRows; }
            set
            {
                numberRows = value;
                OnPropertyChanged(nameof(NumberRows));
            }
        }

        int numberColumns = 5;

        public int NumberColumns
        {
            get { return numberColumns; }
            set
            {
                numberColumns = value;
                OnPropertyChanged(nameof(NumberColumns));
            }
        }

        double e = 2.54;

        /// <summary>
        /// E - width
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

        double d = 12.70d;
        /// <summary>
        /// D - body height
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

        double pinPitchE = 2.54d;
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchE
        {
            get { return pinPitchE; }
            set
            {
                pinPitchE = value;
                OnPropertyChanged(nameof(PinPitchE));
            }
        }

        double pinPitchD = 2.54d;
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchD
        {
            get { return pinPitchD; }
            set
            {
                pinPitchD = value;
                OnPropertyChanged(nameof(PinPitchD));
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

        double l = 1.8d;

        public double L
        {
            get { return l; }
            set
            {
                l = value;
                OnPropertyChanged(nameof(L));
            }
        }

       

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();



                var origin = new XPoint(D, D);

                var rowOffset = 0.5 * (d - (numberColumns - 1) * pinPitchD);
                var colOffset = 0.5 * (e - (numberRows - 1) * pinPitchE);

                int padNumber = 1;

                //balls
                for (int rowIndex = 0; rowIndex < NumberRows; rowIndex++)
                {
                    var y = -0.5 * e + colOffset + rowIndex * pinPitchE;
                    for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
                    {
                        var x = -0.5 * d + rowOffset + colIndex * pinPitchD;

                        var padPrimitive = new Pad
                        {
                            x = origin.X + x,
                            y = origin.Y + y,
                            Width = l,
                            Height = l,
                            CornerRadius = 0.5 * l,
                            drill = b,
                            number = padNumber.ToString(),

                        };

                        padNumber++;

                        var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                        (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;
                        newItems.Add(pad);
                    }
                }




                //body
                CreateBodySilk(newItems, origin);



                return newItems;
            });
        }

        void CreateBodySilk(List<BaseCanvasItem> newItems, XPoint origin)
        {
            var points = new List<XPoint>
            {
                new XPoint(-0.5 * d, -0.5 * e),
                new XPoint(0.5 * d, -0.5 * e),
                new XPoint(0.5 * d, 0.5 * e),
                new XPoint(-0.5 * d, 0.5 * e),
                new XPoint(-0.5 * d, -0.5 * e),
            };

            var primitives = GetLinesFromPoints(points, origin);

            foreach (var primitive in primitives)
            {
                var line = (BoardCanvasItemViewModel)primitive.CreateDesignerItem();
                (line as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SilkscreenTopLayerId;
                newItems.Add(line);
            }
        }
    }
}
