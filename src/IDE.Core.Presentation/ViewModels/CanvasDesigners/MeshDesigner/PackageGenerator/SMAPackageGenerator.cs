using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SMAPackageGenerator : PackageGenerator
    {
        public override string Name => "SMA";


        double e = 2.9d;

        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
            }
        }

        double d1 = 4.6d;

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

        double d = 5.28d;
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



        double l = 1.5d;
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

        double b = 1.6d;
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

        double a = 2.44d;
        /// <summary>
        /// Height of the package
        /// </summary>
        public double A
        {
            get { return a; }
            set
            {
                a = value;
                OnPropertyChanged(nameof(A));
            }
        }

        double a1 = 0.2;
        /// <summary>
        /// Body offset
        /// </summary>
        public double A1
        {
            get { return a1; }
            set
            {
                a1 = value;
                OnPropertyChanged(nameof(A1));
            }
        }

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                var bodyHeight = A - A1;

                //body
                meshItems.Add(new BoxMeshItem
                {
                    Z = A1 + 0.5 * bodyHeight,
                    Width = e,
                    Length = d1,
                    Height = bodyHeight,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Black
                });


                //pad1
                meshItems.Add(new BoxMeshItem
                {
                    X = 0.5 * (D - L),
                    Y = 0,
                    Z = 0.5 * (0.5 * bodyHeight + A1),
                    Width = b,
                    Height = 0.5 * bodyHeight + A1,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    PadNumber = 1,
                    FillColor = XColors.Silver
                });
                //pad2
                meshItems.Add(new BoxMeshItem
                {
                    X = -0.5 * (D - L),
                    Y = 0,
                    Z = 0.5 * (0.5 * bodyHeight + A1),
                    Width = b,
                    Height = 0.5 * bodyHeight + A1,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    PadNumber = 2,
                    FillColor = XColors.Silver
                });

                //diode marking (near pad 2)
                meshItems.Add(new BoxMeshItem
                {
                    X = -0.5 * (d1 - L) + 0.01,
                    Y = 0,
                    Z = A1 + 0.5 * bodyHeight + 0.02,
                    Width = 0.9 * e,
                    Height = bodyHeight,
                    Length = 0.1 * D1,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.WhiteSmoke,
                });

                return meshItems;
            });
        }



    }
}
