using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class AxialFootprintGenerator : FootprintGenerator
    {

        public AxialFootprintGenerator(ILayeredViewModel doc)
            : base(doc)
        {

        }

        public override string Name => "Axial";

        ComponentAppearance appearance;
        public ComponentAppearance Appearance
        {
            get { return appearance; }
            set
            {
                appearance = value;
                OnPropertyChanged(nameof(Appearance));
            }
        }

        ComponentPlacement placement;
        public ComponentPlacement Placement
        {
            get { return placement; }
            set
            {
                placement = value;
                OnPropertyChanged(nameof(Placement));
            }
        }


        bool isDiode;
        public bool IsDiode
        {
            get { return isDiode; }
            set
            {
                isDiode = value;
                OnPropertyChanged(nameof(IsDiode));
            }
        }

        double d = 2.54 * 3;
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

        double d1 = 2.54 * 2;

        /// <summary>
        /// Length of the body cilinder
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

        double l = 1.8;
        /// <summary>
        /// Length of pad 1 in mm
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

        double e = 2.0d;
        /// <summary>
        /// Width of the package
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

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();



                var origin = new XPoint(D, D);
                var padPrimitive = new Pad
                {
                    x = origin.X + 0.5 * (D),
                    y = origin.Y + 0,
                    Width = L,
                    Height = L,
                    CornerRadius = 0.5 * L,
                    drill = b,
                    number = "2",
                };
                var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;
                newItems.Add(pad);

                //pad2

                padPrimitive = new Pad
                {
                    x = origin.X + -0.5 * (D),
                    y = origin.Y + 0,
                    Width = L,
                    Height = L,
                    CornerRadius = 0.5 * L,
                    drill = b,
                    number = "1",
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

            if (isDiode)
            {
                primitives.Add(new LineBoard
                {
                    x1 = origin.X - 0.5 * d1 + 0.5,
                    y1 = origin.Y - 0.5 * e,
                    x2 = origin.X - 0.5 * d1 + 0.5,
                    y2 = origin.Y + 0.5 * e,
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
