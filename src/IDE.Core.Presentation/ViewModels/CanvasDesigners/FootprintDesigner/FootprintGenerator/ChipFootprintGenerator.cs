using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    //all units in mm
    public class ChipFootprintGenerator : FootprintGenerator
    {
        public ChipFootprintGenerator(ILayeredViewModel doc) : base(doc)
        {

        }

        public override string Name => "Chip";

        double l1 = 0.7;
        /// <summary>
        /// Length of pad 1 in mm
        /// </summary>
        public double L1
        {
            get { return l1; }
            set
            {
                l1 = value;
                OnPropertyChanged(nameof(L1));
            }
        }

        //double l2 = 0.3;
        ///// <summary>
        ///// Length of pad 2 in mm
        ///// </summary>
        //public double L2
        //{
        //    get { return l2; }
        //    set
        //    {
        //        l2 = value;
        //        OnPropertyChanged(nameof(L2));
        //    }
        //}

        double d = 2.6;
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

        //double a = 0.7;
        ///// <summary>
        ///// Height of the package
        ///// </summary>
        //public double A
        //{
        //    get { return a; }
        //    set
        //    {
        //        a = value;
        //        OnPropertyChanged(nameof(A));
        //    }
        //}

        double e = 1.3d;
        /// <summary>
        /// Width of the Footprint
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

        public override Task<List<BaseCanvasItem>> GenerateFootprint()
        {
            return Task.Run(() =>
            {
                var newItems = new List<BaseCanvasItem>();



                var origin = new XPoint(D, D);
                var padPrimitive = new Smd
                {
                    x = origin.X + 0.5 * (D - L1),
                    y = origin.Y + 0,
                    Width = L1,
                    Height = E,
                    number = "1",
                };
                var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;
                newItems.Add(pad);

                //pad2

                padPrimitive = new Smd
                {
                    x = origin.X + -0.5 * (D - L1),
                    y = origin.Y + 0,
                    Width = L1,
                    Height = E,
                    number = "2",
                };
                pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

                newItems.Add(pad);
                ////body
                //newItems.Add(new BoxMeshItem
                //{
                //    X = 0,
                //    Y = 0,
                //    Z = 0.5 * A + baseHeight,
                //    Width = E,
                //    Height = A,
                //    Length = D - L1 - L2,
                //    CanEdit = false,
                //    IsPlaced = false,
                //    //PadNumber = 2,
                //    FillColor = Colors.Black
                //});


                return newItems;
            });
        }
    }
}
