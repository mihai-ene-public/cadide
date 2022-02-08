using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SOICFootprintGenerator : FootprintGenerator
    {
        public SOICFootprintGenerator(ILayeredViewModel doc) : base(doc)
        {

        }

        public override string Name => "SOP / SOIC";


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

        double e = 1.27d;
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

        double d1 = 4;

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

        double d = 6.2d;
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

        double ee = 5.0d;
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

        //double l = 0.5d;
        ///// <summary>
        ///// Length of landing pad in mm
        ///// </summary>
        //public double L
        //{
        //    get { return l; }
        //    set
        //    {
        //        l = value;
        //        OnPropertyChanged(nameof(L));
        //    }
        //}

        double b = 0.48d;
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

        //double a = 1.75;
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

        //double a1 = 0.23;
        ///// <summary>
        ///// Body offset
        ///// </summary>
        //public double A1
        //{
        //    get { return a1; }
        //    set
        //    {
        //        a1 = value;
        //        OnPropertyChanged(nameof(A1));
        //    }
        //}

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
                    var rot = 0.0d;
                    var width = 0.5 * (D - D1);
                    var height = B;

                    switch (padEdgeIndex)
                    {
                        case 0://vertical -down
                            {
                                x = -(0.5 * D1 + 0.5 * width);
                                y = -0.5 * D1 + padOffset + indexInEdge * E;
                                break;
                            }
                        case 1://vertical up
                            {
                               // rot = 180;
                                x = 0.5 * D1 + 0.5 * width;
                                y = -0.5 * D1 + padOffset + (edgeSize - 1 - indexInEdge) * E;
                                break;
                            }
                    }

                    var padPrimitive = new Smd
                    {
                        x = origin.X + x,
                        y = origin.Y + y,
                        rot = rot,
                        Width = width,
                        Height = height,
                        number = (padIndex + 1).ToString()
                    };
                    var pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                    (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;

                    newItems.Add(pad);
                }

                ////pin 1 mark
                //newItems.Add(new CylinderMeshItem
                //{
                //    X = -0.5 * D1 + 0.5 * padOffset,
                //    Y = -0.5 * EE + padOffset,
                //    Z = A - 0.1,
                //    Radius = 0.2,
                //    Height = 0.15,
                //    CanEdit = false,
                //    IsPlaced = false,
                //    FillColor = Colors.WhiteSmoke
                //});

                return newItems;
            });
        }


    }
}
