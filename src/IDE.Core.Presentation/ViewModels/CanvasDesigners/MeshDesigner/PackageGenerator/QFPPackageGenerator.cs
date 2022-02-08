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
    public class QFPPackageGenerator : PackageGenerator
    {
        public override string Name => "QFP";


        int numberPads = 32;
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

        double e = 0.8d;
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

        double d1 = 7.1;

        /// <summary>
        /// Body size
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

        double d = 9.25d;
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

        double l = 0.75d;
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

        double b = 0.45d;
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

        double a = 1.2;
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

        double a1 = 0.15;
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

                CreateBody(meshItems);

                CreatePads(meshItems);

                return meshItems;
            });
        }

        void CreateBody(List<BaseMeshItem> meshItems)
        {
            meshItems.Add(new BoxMeshItem
            {
                Z = A1 + 0.5 * (A - A1),
                Width = d1,
                Height = A - A1,
                Length = d1,
                CanEdit = false,
                IsPlaced = false,
                RotationZ = 90,
                FillColor = XColors.Black
            });


        }

        void CreatePads(List<BaseMeshItem> meshItems)
        {
            var edgeSize = NumberPads / 4;//32/4=8
            var padOffset = 0.5 * (D1 - (edgeSize - 1) * E);


            for (int padIndex = 0; padIndex < NumberPads; padIndex++)
            {
                var padEdgeIndex = padIndex / edgeSize;
                var indexInEdge = padIndex % edgeSize;

                var x = 0.0d;
                var y = 0.0d;
                var z = 0.5 * a1;
                var rot = 0.0d;

                switch (padEdgeIndex)
                {
                    case 0://vertical -down
                        {
                            rot = 180;
                            x = -0.5 * D1;
                            y = -0.5 * D1 + padOffset + indexInEdge * E;
                            break;
                        }
                    case 1://horizontal-right
                        {
                            rot = 90;
                            x = -0.5 * D1 + padOffset + indexInEdge * E;
                            y = 0.5 * D1;

                            break;
                        }
                    case 2://vertical up
                        {
                            rot = 0;
                            x = 0.5 * D1;
                            y = -0.5 * D1 + padOffset + (edgeSize - 1 - indexInEdge) * E;
                            break;
                        }
                    case 3://horizontal-left
                        {
                            rot = 270;
                            x = -0.5 * D1 + padOffset + (edgeSize - 1 - indexInEdge) * E;
                            y = -0.5 * D1;
                            break;
                        }
                }

                meshItems.Add(CreatePad(new XPoint3D(x, y, z), rot, padIndex + 1));
            }

            //pin 1 mark
            meshItems.Add(new CylinderMeshItem
            {
                X = -0.5 * d1 + 0.5 * padOffset,
                Y = -0.5 * d1 + padOffset,
                Z = A - 0.1,
                Radius = 0.2,
                Height = 0.15,
                CanEdit = false,
                IsPlaced = false,
                FillColor = XColors.WhiteSmoke
            });
        }

        BaseMeshItem CreatePad(XPoint3D position, double rot, int padNumber)
        {
            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            IMeshModel padMesh = null;

            var path = CreateGullWingPathPin();

            var section = GeometryTools.GetRectangleSectionPoints(b, a1);

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
            var pinThickness = a1;
            var bendRadius = 0.55 * pinThickness;
            var bodyHeight = (A - A1);
            var pathHeight = 0.5 * bodyHeight;
            double upperSegmentLength = 0.5 * (d - d1 - 2 * l - 4 * bendRadius);//0.5 * (d - d1) - l;// E - E1 - 2 * L - 2 * bendRadius;
            var lowerSegmentLength = L;


            var path = GeometryTools.CreateGullWingPathPin(pathHeight, upperSegmentLength, lowerSegmentLength, bendRadius);

            return path;
        }

    }
}
