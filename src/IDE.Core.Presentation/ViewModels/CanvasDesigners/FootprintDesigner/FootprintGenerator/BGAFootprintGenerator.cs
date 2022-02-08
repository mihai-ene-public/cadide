using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class BGAFootprintGenerator : FootprintGenerator
    {
        public BGAFootprintGenerator(ILayeredViewModel doc)
                   : base(doc)
        {

        }


        public override string Name => "BGA";


        int numberRows = 4;

        public int NumberRows
        {
            get { return numberRows; }
            set
            {
                numberRows = value;
                OnPropertyChanged(nameof(NumberRows));
            }
        }

        int numberColumns = 4;

        public int NumberColumns
        {
            get { return numberColumns; }
            set
            {
                numberColumns = value;
                OnPropertyChanged(nameof(NumberColumns));
            }
        }

        double e = 3.0d;

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

        double d1 = 0.5;

        /// <summary>
        /// d - pitch between rows
        /// </summary>
        public double D1 //d
        {
            get { return d1; }
            set
            {
                d1 = value;
                OnPropertyChanged(nameof(D1));
            }
        }

        double d = 3.0d;
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

        double ee = 0.5d;
        /// <summary>
        /// Lower e - pitch between columns
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



        double b = 0.3d;
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

                var rowOffset = 0.5 * (E - (numberRows - 1) * ee);
                var colOffset = 0.5 * (d - (numberColumns - 1) * d1);

                int padNumber = 1;

                //balls
                for (int rowIndex = 0; rowIndex < NumberRows; rowIndex++)
                {
                    var z = 0.5 * b;
                    var y = -0.5 * d + colOffset + rowIndex * d1;
                    for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
                    {
                        var x = -0.5 * e + rowOffset + colIndex * ee;

                        var padPrimitive = new Smd
                        {
                            x = origin.X + x,
                            y = origin.Y + y,
                            Width = b,
                            Height = b,
                            CornerRadius = 0.5 * b,
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
            var primitives = new List<LayerPrimitive>();
            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * e,
                y1 = origin.Y - 0.5 * d,
                x2 = origin.X + 0.5 * e,
                y2 = origin.Y - 0.5 * d,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * e,
                y1 = origin.Y + 0.5 * d,
                x2 = origin.X + 0.5 * e,
                y2 = origin.Y + 0.5 * d,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X - 0.5 * e,
                y1 = origin.Y - 0.5 * d,
                x2 = origin.X - 0.5 * e,
                y2 = origin.Y + 0.5 * d,
            });

            primitives.Add(new LineBoard
            {
                x1 = origin.X + 0.5 * e,
                y1 = origin.Y - 0.5 * d,
                x2 = origin.X + 0.5 * e,
                y2 = origin.Y + 0.5 * d,
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
