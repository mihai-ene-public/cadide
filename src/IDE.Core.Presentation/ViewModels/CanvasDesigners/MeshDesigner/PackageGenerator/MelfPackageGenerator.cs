using IDE.Core.Designers;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class MelfPackageGenerator : PackageGenerator
    {
        public override string Name => "MELF";

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



        double d = 3.5;
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



        double e = 1.5d;
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

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();


                //pad1
                meshItems.Add(new CylinderMeshItem
                {
                    X = -0.5 * (D - 2 * L),
                    Z = 0.5 * E,
                    Height = L,
                    Radius = 0.5 * E,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = -1,
                    DirectionY = 0,
                    DirectionZ = 0,
                    FillColor = XColors.Silver,
                    CanEdit = false,
                    IsPlaced = false
                });
                //pad2
                meshItems.Add(new CylinderMeshItem
                {
                    X = 0.5 * (D - 2 * L),
                    Z = 0.5 * E,
                    Height = L,
                    Radius = 0.5 * E,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 1,
                    DirectionY = 0,
                    DirectionZ = 0,
                    FillColor = XColors.Silver,
                    CanEdit = false,
                    IsPlaced = false
                });
                //body
                meshItems.Add(new CylinderMeshItem
                {
                    X = -0.5 * (D - 2 * L),
                    Z = 0.5 * E,
                    Height = D - 2 * L,
                    Radius = 0.5 * E,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 1,
                    DirectionY = 0,
                    DirectionZ = 0,
                    FillColor = XColors.Black,
                    CanEdit = false,
                    IsPlaced = false
                });


                return meshItems;
            });
        }
    }
}
