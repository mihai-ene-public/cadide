using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class AxialPackageGenerator : PackageGenerator
    {
        public override string Name => "Axial";

        ComponentAppearance appearance;
        public ComponentAppearance Appearance
        {
            get { return appearance; }
            set
            {
                appearance = value;
                OnPropertyChanged(nameof(Appearance));
            }
        }

        ComponentPlacement placement;
        public ComponentPlacement Placement
        {
            get { return placement; }
            set
            {
                placement = value;
                OnPropertyChanged(nameof(Placement));
            }
        }


        bool isDiode;
        public bool IsDiode
        {
            get { return isDiode; }
            set
            {
                isDiode = value;
                OnPropertyChanged(nameof(IsDiode));
            }
        }

        double d = 2.54 * 3;
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

        double d1 = 2.54 * 2;

        /// <summary>
        /// Length of the body cilinder
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

        double a = 4;
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

        double e = 2.0d;
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

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                var bodyColor = XColors.Goldenrod;

                if (IsDiode || Appearance == ComponentAppearance.Diode)
                {
                    //diode marking
                    meshItems.Add(new CylinderMeshItem
                    {
                        X = -0.5 * D1 + 0.01,
                        Z = A - 0.5 * E,
                        Height = 0.1 * D1,
                        Radius = 0.5 * E * 1.01,
                        ShowTopCap = true,
                        ShowBaseCap = true,
                        DirectionX = 1,
                        DirectionY = 0,
                        DirectionZ = 0,
                        FillColor = XColors.WhiteSmoke,
                        CanEdit = false,
                        IsPlaced = false
                    });

                    bodyColor = XColors.Black;
                }

                //body
                meshItems.Add(new CylinderMeshItem
                {
                    X = -0.5 * D1,
                    Z = A - 0.5 * E,
                    Height = D1,
                    Radius = 0.5 * E,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 1,
                    DirectionY = 0,
                    DirectionZ = 0,
                    FillColor = bodyColor,
                    CanEdit = false,
                    IsPlaced = false
                });

                //pad 1
                meshItems.Add(CreatePad(new XPoint3D(0.5 * D1, 0, A - 0.5 * E), 0, 1));

                //pad 2
                meshItems.Add(CreatePad(new XPoint3D(-0.5 * D1, 0, A - 0.5 * E), 180, 2));

                return meshItems;
            });
        }

        BaseMeshItem CreatePad(XPoint3D position, double rot, int padNumber)
        {
            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            IMeshModel padMesh = null;

            var path = new List<XPoint3D>
            {
                new XPoint3D(0,0,0),
            };

            var curveStart = 0.5 * (D - D1) - b;
            // var x = B;
            for (int i = 10; i >= 0; i--)
            {
                var t = i * Math.PI / 2 / 10;
                path.Add(new XPoint3D(curveStart + B * Math.Cos(t), 0, B - B * Math.Sin(t)));
            }
            path.Add(new XPoint3D(B + curveStart, 0, A - 0.5 * E));

            dispatcher.RunOnDispatcher(() => padMesh = meshHelper.CreateTube(path, b, 32, true));

            var pad = new SolidBodyMeshItem
            {
                Model = padMesh,
                FillColor = XColors.Silver,
                PadNumber = padNumber,
                RotationZ = rot,
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                IsPlaced = false,
                CanEdit = false
            };

            return pad;
        }


    }
}
