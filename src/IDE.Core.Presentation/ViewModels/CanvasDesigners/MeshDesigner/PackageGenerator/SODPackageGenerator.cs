using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SODPackageGenerator : PackageGenerator
    {
        public override string Name => "SOD";


        double e = 1.8d;

        public double E
        {
            get { return e; }
            set
            {
                e = value;
                OnPropertyChanged(nameof(E));
            }
        }

        double d1 = 3.5;

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

        double d = 4.5d;
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



        double l = 0.3d;
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

        double a = 1.3;
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

        double a1 = 0.23;
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

        const double pinThickness = 0.12d;

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //body
                meshItems.Add(new BoxMeshItem
                {
                    Z = A1 + 0.5 * (A - A1),
                    Width = e,
                    Length = d1,
                    Height = A - A1,
                    CanEdit = false,
                    IsPlaced = false,
                    //RotationZ = 90,
                    FillColor = XColors.Black
                });

                var z = 0.5 * pinThickness;//A1 + 0.5 * (A - A1);
                //var padOffset = 0.5 * (D - (edgeSize - 1) * EE);

                meshItems.Add(CreatePad(new XPoint3D(-0.5 * d1, 0, z), 180, 1));

                meshItems.Add(CreatePad(new XPoint3D(0.5 * d1, 0, z), 0, 2));

                return meshItems;
            });
        }

        BaseMeshItem CreatePad(XPoint3D position, double rot, int padNumber)
        {
            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            IMeshModel padMesh = null;

            var path = CreateGullWingPathPin();

            var section = new List<XPoint>()
                    {
                        new XPoint(-0.5 * B, -0.5 * pinThickness),
                        new XPoint(-0.5 * B,  0.5 * pinThickness),
                        new XPoint( 0.5 * B,  0.5 * pinThickness),
                        new XPoint( 0.5 * B, -0.5 * pinThickness),
                        new XPoint(-0.5 * B, -0.5 * pinThickness),
                    };

            dispatcher.RunOnDispatcher(() => padMesh = meshHelper.CreateTube(path, section, true));

            var pad = new SolidBodyMeshItem
            {
                Model = padMesh,
                FillColor = XColors.Silver,
                PadNumber = padNumber,
                RotationX = 180,
                RotationZ = rot,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                IsPlaced = false,
                CanEdit = false
            };

            return pad;
        }

        IList<XPoint3D> CreateGullWingPathPin()
        {
            var bendRadius = 0.55 * pinThickness;
            var bodyHeight = (A - A1);
            var pathHeight = A1 - 0.5 * pinThickness + 0.5 * bodyHeight;
            double upperSegmentLength = d - d1 - 2 * L - 2 * bendRadius;
            var lowerSegmentLength = L;

            var path = GeometryTools.CreateGullWingPathPin(pathHeight, upperSegmentLength, lowerSegmentLength, bendRadius);

            return path;
        }

    }

}
