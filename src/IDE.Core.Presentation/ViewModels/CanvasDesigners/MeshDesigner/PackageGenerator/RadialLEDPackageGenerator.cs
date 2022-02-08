using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class RadialLEDPackageGenerator : PackageGenerator
    {
        public override string Name => "Radial LED";


        double a = 11.6;
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


        double d = 5;
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

        double ee = 2.54d;
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

        XColor _bodyColor = XColors.Red;
        public XColor BodyColor
        {
            get { return _bodyColor; }
            set
            {
                _bodyColor = value;
                OnPropertyChanged(nameof(BodyColor));
            }
        }

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                CreateBody(meshItems);

                //pad1
                meshItems.Add(new CylinderMeshItem
                {
                    X = -0.5 * EE,
                    Z = 0.01d,
                    Height = L + 0.01d,
                    Radius = 0.5 * B,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = -1,
                    FillColor = XColors.Silver,
                    PadNumber = 1,
                    CanEdit = false,
                    IsPlaced = false
                });
                //pad2
                meshItems.Add(new CylinderMeshItem
                {
                    X = 0.5 * EE,
                    Z = 0.01d,
                    Height = L + 0.01d,
                    Radius = 0.5 * B,
                    ShowTopCap = true,
                    ShowBaseCap = true,
                    DirectionX = 0,
                    DirectionY = 0,
                    DirectionZ = -1,
                    FillColor = XColors.Silver,
                    PadNumber = 2,
                    CanEdit = false,
                    IsPlaced = false
                });



                return meshItems;
            });
        }

        void CreateBody(List<BaseMeshItem> meshItems)
        {
            //body
            var baseHeight = 1.0d;
            var bodyColor = XColor.FromArgb(127, _bodyColor.R, _bodyColor.G, _bodyColor.B);

            var cyl = new CylinderMeshItem
            {
                Z = baseHeight,
                Height = A - L - 0.5d * d - baseHeight,
                Radius = 0.5 * d,
                ShowTopCap = false,
                ShowBaseCap = false,
                DirectionX = 0,
                DirectionY = 0,
                DirectionZ = 1,
                FillColor = bodyColor,
                CanEdit = false,
                IsPlaced = false
            };
            meshItems.Add(cyl);
            //meshItems.Add(new SphereMeshItem
            //{
            //    Z = a - l - 0.5 * d,
            //    Radius = 0.5 * d - 0.01,
            //    FillColor = bodyColor,
            //    IsPlaced = false,
            //    CanEdit = false,
            //});
            meshItems.Add(CreateSemiSphere(a - l - 0.5 * d, 0.5 * d, bodyColor, cyl.ThetaDivisions));

            meshItems.Add(CreateBase(bodyColor, baseHeight));
        }

        BaseMeshItem CreateBase(XColor color, double height)
        {
            const int steps = 32;

            var points = new List<XPoint>();

            var canRadius = 0.5 * (d + 1.0);

            var angle = Math.Acos(0.5 * d / canRadius);
            var angleStart = Math.PI - angle;
            var totalAngle = 2 * Math.PI - 2 * angle;

            points.AddRange(GeometryTools.GetCirclePoints(new XPoint(0, 0), steps, totalAngle, angleStart, canRadius, false));
            points.Add(points[0]);

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutline(points, -height));

            return new SolidBodyMeshItem
            {
                Model = mesh,
                FillColor = color,
                IsPlaced = false,
                CanEdit = false
            };
        }

        private BaseMeshItem CreateSemiSphere(double height, double radius, XColor color, int thetaDiv)
        {
            const int steps = 32;

            var points = new List<XPoint>();

            var totalAngle = 0.5 * Math.PI;

            points.AddRange(GeometryTools.GetCirclePoints(new XPoint(0, 0), steps, totalAngle, 0.0d, radius, true));

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.RevolveOutline(points, new XPoint3D(0, 0, 0), new XVector3D(0, 0, -1), thetaDiv));

            return new SolidBodyMeshItem
            {
                Z = height,
                Model = mesh,
                FillColor = color,
                IsPlaced = false,
                CanEdit = false
            };
        }
    }

}
