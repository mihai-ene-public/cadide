using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class DIPFootprintGenerator : FootprintGenerator
    {
        public DIPFootprintGenerator(ILayeredViewModel doc) : base(doc)
        {

        }
        public override string Name => "DIP";

        int numberPads = 8;
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

        double e = 2.54d;
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

        double d1 = 6.4;

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

        double d = 2.54 * 3d;
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

        double ee = 8.8d;
        /// <summary>
        /// Body length
        /// </summary>
        public double EE//D
        {
            get { return ee; }
            set
            {
                ee = value;
                OnPropertyChanged(nameof(EE));
            }
        }

        double l = 1.8d;
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
                var edgeSize = NumberPads / 2;//8/2=4
                var padOffset = 0.5 * (EE - (edgeSize - 1) * E);

                for (int padIndex = 0; padIndex < NumberPads; padIndex++)
                {

                    var padEdgeIndex = padIndex / edgeSize;
                    var indexInEdge = padIndex % edgeSize;

                    var x = 0.0d;
                    var y = 0.0d;
                    var width = l;
                    var height = l;

                    switch (padEdgeIndex)
                    {
                        case 0://vertical -down
                            {
                                x = -(0.5 * d);// + 0.5 * width);
                                y = -0.5 * ee + padOffset + indexInEdge * E;
                                break;
                            }
                        case 1://vertical up
                            {
                                x = 0.5 * d;// + 0.5 * width;
                                y = -0.5 * ee + padOffset + (edgeSize - 1 - indexInEdge) * E;
                                break;
                            }
                    }

                    var padPrimitive = new Pad
                    {
                        x = origin.X + x,
                        y = origin.Y + y,
                        //rot = rot,
                        Width = width,
                        Height = height,
                        CornerRadius = 0.5 * width,
                        drill = b,
                        number = (padIndex + 1).ToString()
                    };
                    var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                    (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

                    newItems.Add(pad);
                }

                //body
                CreateBodySilk(newItems, origin);

                return newItems;
            });
        }

        void CreateBodySilk(List<BaseCanvasItem> newItems, XPoint origin)
        {
            var points = new List<XPoint>();

            points.Add(new XPoint(-0.5 * d1, -0.5 * ee));
            points.Add(new XPoint(-0.5 * d1, 0.5 * ee));
            points.Add(new XPoint(0.5 * d1, 0.5 * ee));
            points.Add(new XPoint(0.5 * d1 , -0.5 * ee));
            points.Add(new XPoint(-0.5 * d1, -0.5 * ee));

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
