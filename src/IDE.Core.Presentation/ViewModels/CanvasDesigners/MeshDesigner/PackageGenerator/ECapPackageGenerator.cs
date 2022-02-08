using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IDE.Documents.Views
{
    public class ECapPackageGenerator : PackageGenerator
    {
        public override string Name => "E-Cap";

        double d1 = 10.5d;

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

        double d = 12.0d;
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


        double l = 3.5d;
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

        double b = 0.63d;
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

        double a = 10.5;
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

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //plastic body
                meshItems.Add(CreateBody());

                //pads
                meshItems.Add(new BoxMeshItem
                {
                    X = -0.5 * (D - L),
                    Y = 0,
                    Z = 0.2,
                    Width = b,
                    Height = 0.3,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Silver
                });
                meshItems.Add(new BoxMeshItem
                {
                    X = 0.5 * (D - L),
                    Y = 0,
                    Z = 0.2,
                    Width = b,
                    Height = 0.3,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Silver
                });

                //can
                meshItems.Add(new CylinderMeshItem
                {
                    // X = -0.5 * D + 0.01,
                    Z = 0.2,
                    Height = A - 0.2,
                    Radius = 0.5 * d1 - 0.3,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = 1,
                    FillColor = XColors.Silver,
                    CanEdit = false,
                    IsPlaced = false
                });

                return meshItems;
            });
        }

        BaseMeshItem CreateBody()
        {
            var points = new List<XPoint>();

            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, 0.5 * d1));
            points.Add(new XPoint(0.5 * d1, 0.5 * d1));
            points.Add(new XPoint(0.5 * d1, -0.5 * d1));
            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, -0.5 * d1));
            points.Add(new XPoint(-0.5 * d1, -0.5 * d1 + 0.25 * d1));
            points.Add(new XPoint(-0.5 * d1, 0.5 * d1 - 0.25 * d1));
            points.Add(new XPoint(-0.5 * d1 + 0.25 * d1, 0.5 * d1));

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutline(points, -1.2));

            return new SolidBodyMeshItem
            {
                Model = mesh,
                FillColor = XColors.Black,
                Z = 0.1,
                IsPlaced = false,
                CanEdit = false
            };
        }
    }
}
