using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class RadialGenericRoundFootprintGenerator : FootprintGenerator
    {
        public RadialGenericRoundFootprintGenerator(ILayeredViewModel doc)
          : base(doc)
        {

        }


        public override string Name => "Radial (Round)";





        double d = 10;
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

        double ee = 5.08d;
        /// <summary>
        /// e - pin pitch
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

        double l = 1.8;
        /// <summary>
        /// Length of pad in mm
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

        //double e = 1.5d;
        ///// <summary>
        ///// Width of the package
        ///// </summary>
        //public double E
        //{
        //    get { return e; }
        //    set
        //    {
        //        e = value;
        //        OnPropertyChanged(nameof(E));
        //    }
        //}

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();



                var origin = new XPoint(D, D);
                var padPrimitive = new Pad
                {
                    x = origin.X + 0.5 * ee,
                    y = origin.Y,
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
                    x = origin.X + -0.5 * ee,
                    y = origin.Y,
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
            primitives.Add(new ArcBoard
            {
                StartPoint = new XPoint(origin.X - 0.5 * d, origin.Y),
                EndPoint = new XPoint(origin.X - 0.5 * d, origin.Y - 0.01),
                BorderWidth = 0.2,
                IsLargeArc = true,
                SizeDiameter = d * 0.5//it seems it is in fact radius
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
