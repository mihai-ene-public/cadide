using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IDE.Documents.Views
{
    public class CrystalSMDPackageGenerator : PackageGenerator
    {
        public override string Name => "Crystal SMD";

        double d1 = 11.7d;

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

        double d = 13.2d;
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

        double e = 5d;
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

        double l = 4d;
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

        double a = 4.3;
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
                meshItems.Add(new BoxMeshItem
                {
                    X = 0,
                    Y = 0,
                    Z = 0.51 * 0.75,
                    Width = E,
                    Height = 0.75,
                    Length = D1,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Black
                });

                //pads
                meshItems.Add(new BoxMeshItem
                {
                    X = -0.5 * (D - L),
                    Y = 0,
                    Z = 0.2,
                    Width = b,
                    Height = 0.4,
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
                    Height = 0.4,
                    Length = L,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Silver
                });

                meshItems.Add(CreateCan());

                return meshItems;
            });
        }

        BaseMeshItem CreateCan()
        {
            const int steps = 18;
            const double padding = 0.5d;

            var points = new List<XPoint>();

            var centerDist = 0.5 * D1 - padding - 0.5 * (E - 2 * padding);
            var canRadius = 0.5 * (E - 2 * padding);

            points.AddRange(GeometryTools.GetCircleSegment(new XPoint(-centerDist, 0), steps, Math.PI, 0.5 * Math.PI, canRadius));
            points.AddRange(GeometryTools.GetCircleSegment(new XPoint(centerDist, 0), steps, Math.PI, -0.5 * Math.PI, canRadius));
            points.Add(points[0]);

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutline(points, -A));

            return new SolidBodyMeshItem
            {
                Model = mesh,
                FillColor = XColors.Silver,
                Z = 0.1,
                IsPlaced = false,
                CanEdit = false
            };
        }

       
    }
}
