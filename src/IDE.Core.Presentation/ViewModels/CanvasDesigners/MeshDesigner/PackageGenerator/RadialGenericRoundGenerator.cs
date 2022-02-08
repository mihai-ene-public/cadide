using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class RadialGenericRoundPackageGenerator : PackageGenerator
    {
        public override string Name => "Radial (Round)";


        double a = 14;
        /// <summary>
        /// Height of the package + lead length
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

        double l = 3;
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


        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //body
                meshItems.Add(new CylinderMeshItem
                {
                    //X = -0.5 * (D - 2 * L),
                    //Z = 0.5 * E,
                    Height = A - L,
                    Radius = 0.5 * D,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = 1,
                    FillColor = XColors.Black,
                    CanEdit = false,
                    IsPlaced = false
                });

                //pad1
                meshItems.Add(new CylinderMeshItem
                {
                    X = -0.5 * EE,
                    //Z = 0.5 * E,
                    Height = L,
                    Radius = 0.5 * B,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = -1,
                    FillColor = XColors.Silver,
                    CanEdit = false,
                    IsPlaced = false
                });
                //pad2
                meshItems.Add(new CylinderMeshItem
                {
                    X = 0.5 * EE,
                   // Z = 0.5 * E,
                    Height = L,
                    Radius = 0.5 * B,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = -1,
                    FillColor = XColors.Silver,
                    CanEdit = false,
                    IsPlaced = false
                });



                return meshItems;
            });
        }
    }

}
