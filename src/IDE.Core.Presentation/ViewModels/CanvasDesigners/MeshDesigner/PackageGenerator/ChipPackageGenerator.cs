using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    //all units in mm
    public class ChipPackageGenerator : PackageGenerator
    {
        public override string Name => "Chip";

        double l = 0.3;
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


        double d = 1.9;
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

        double a = 0.7;
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

        double e = 1.0d;
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

        bool isCapacitor = false;
        public bool IsCapacitor
        {
            get { return isCapacitor; }
            set
            {
                isCapacitor = value;
                OnPropertyChanged(nameof(IsCapacitor));
            }
        }

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                var baseHeight = 0.03d;

                //pad1
                meshItems.Add(new BoxMeshItem
                {
                    X = 0.5 * (D - L),
                    Y = 0,
                    Z = 0.5 * A + baseHeight,
                    Width = E,
                    Height = A,
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
                    Z = 0.5 * A + baseHeight,
                    Width = E,
                    Height = A,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    PadNumber = 2,
                    FillColor = XColors.Silver
                });
                //body
                meshItems.Add(new BoxMeshItem
                {
                    X = 0,
                    Y = 0,
                    Z = 0.5 * A + baseHeight,
                    Width = E,
                    Height = A,
                    Length = D - L - L,
                    CanEdit = false,
                    IsPlaced = false,
                    //PadNumber = 2,
                    FillColor = IsCapacitor ? XColors.Orange : XColors.Black
                });


                return meshItems;
            });
        }
    }

}
