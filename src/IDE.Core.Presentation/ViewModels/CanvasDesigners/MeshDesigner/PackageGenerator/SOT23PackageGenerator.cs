using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Common;
using System.Linq;

namespace IDE.Documents.Views
{
    public class SOT23PackageGenerator : PackageGenerator
    {
        public override string Name => "SOT23";


        int numberPads = 3;
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

        public List<int> NumberPadsAvailable => new List<int> { 3, 5 };

        double e = 2.3d;
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

        double e1 = 1.4;

        /// <summary>
        /// Body width
        /// </summary>
        public double E1
        {
            get { return e1; }
            set
            {
                e1 = value;
                OnPropertyChanged(nameof(E1));
            }
        }

        double d = 3.0d;
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

        double ee = 1.03d;
        /// <summary>
        /// Pin pitch
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

        double l = 0.25d;
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

        double b = 0.5d;
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

        double a = 1.12d;
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

        double a1 = 0.1;
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

        const double pinThickness = 0.16d;

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //body
                meshItems.Add(new BoxMeshItem
                {
                    Z = A1 + 0.5 * (A - A1),
                    Width = e1,
                    Height = A - A1,
                    Length = d,
                    CanEdit = false,
                    IsPlaced = false,
                    //RotationZ = 90,
                    FillColor = XColors.Black
                });

                var z = 0.5 * pinThickness;//A1 + 0.5 * (A - A1);
                var edgeSize = 3;
                var padOffset = 0.5 * (D - (edgeSize - 1) * EE);

                switch (numberPads)
                {
                    case 3:
                        //slot 1
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset, 0.5 * E1, z), 90, 1));
                        //slot 3
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + 2 * EE, 0.5 * E1, z), 90, 2));
                        //slot 5
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + EE, -0.5 * E1, z), -90, 3));
                        break;

                    case 5:
                        //slot 1
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset, 0.5 * E1, z), 90, 1));
                        //slot 2
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + EE, 0.5 * E1, z), 90, 2));
                        //slot 3
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + 2 * EE, 0.5 * E1, z), 90, 3));
                        //slot 4
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + 2 * EE, -0.5 * E1, z), -90, 4));
                        //slot 6
                        meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset, -0.5 * E1, z), -90, 5));
                        break;

                        //case 6:
                        //    break;
                }

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
                        //new XPoint(-0.5 * B, -0.5 * pinThickness),
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
            var path = new List<XPoint3D>();


            var bendRadius = 0.55 * pinThickness;
            var bodyHeight = (A - A1);
            var pathHeight = A1 - 0.5 * pinThickness + 0.5 * bodyHeight;
            double upperSegmentLength = E - E1 - 2 * L - 2 * bendRadius;
            if (upperSegmentLength < 0.0d)
                upperSegmentLength = 0.0d;
            var lowerSegmentLength = L;


            path.Add(new XPoint3D(0, 0, pathHeight));

            var bend = GeometryTools.GetCirclePoints(new XPoint(upperSegmentLength, -bendRadius + pathHeight), 10, 0.5 * Math.PI, 0.5 * Math.PI, bendRadius, false);

            path.AddRange(bend.Select(p => new XPoint3D(p.X, 0.0d, p.Y)));

            bend = GeometryTools.GetCirclePoints(new XPoint(upperSegmentLength + 2 * bendRadius, bendRadius),
                                                  10,
                                                  0.5 * Math.PI,
                                                  Math.PI,
                                                  bendRadius,
                                                  true);

            path.AddRange(bend.Select(p => new XPoint3D(Math.Round(p.X, 4), 0.0d, Math.Round(p.Y, 4))));

            path.Add(new XPoint3D(upperSegmentLength + 2 * bendRadius + lowerSegmentLength, 0, 0));

            path.Reverse();

            return path;
        }
    }

}
