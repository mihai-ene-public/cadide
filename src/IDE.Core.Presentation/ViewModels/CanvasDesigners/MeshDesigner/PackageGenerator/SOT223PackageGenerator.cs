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
    public class SOT223PackageGenerator : PackageGenerator
    {
        public override string Name => "SOT223";


        int numberPads = 4;
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

        public List<int> NumberPadsAvailable => new List<int> { 4 };

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

        double b1 = 1.2d;
        /// <summary>
        /// Pad width in mm
        /// </summary>
        public double B1
        {
            get { return b1; }
            set
            {
                b1 = value;
                OnPropertyChanged(nameof(B1));
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
                    FillColor = XColors.Black
                });

                var z = 0.5 * pinThickness;//A1 + 0.5 * (A - A1);
                var edgeSize = 3;
                var padOffset = 0.5 * (D - (edgeSize - 1) * EE);
                var posY = 0.5 * e1;

              
                for (int i = 0; i < numberPads - 1; i++)
                {
                    meshItems.Add(CreatePad(new XPoint3D(-0.5 * D + padOffset + i * EE, posY, z), 90, i + 1, b, pinThickness));
                }

                //pin tab
                meshItems.Add(CreatePad(new XPoint3D(0, -posY, z), -90, numberPads, b1, pinThickness));


                return meshItems;
            });
        }

        BaseMeshItem CreatePad(XPoint3D position, double rot, int padNumber, double width, double thickness)
        {
            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            IMeshModel padMesh = null;


            var path = CreateGullWingPathPin();

            var section = GeometryTools.GetRectangleSectionPoints(width, thickness);

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
            double upperSegmentLength = E - E1 - 2 * L - 2 * bendRadius;
            var lowerSegmentLength = L;

            var path = GeometryTools.CreateGullWingPathPin(pathHeight, upperSegmentLength, lowerSegmentLength, bendRadius);

            return path;
        }
    }

}
