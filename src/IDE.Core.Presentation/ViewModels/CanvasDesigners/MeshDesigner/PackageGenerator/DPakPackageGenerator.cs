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
    public class DPakPackageGenerator : PackageGenerator
    {
        public override string Name => "D-Pak";

        double bodyWidth = 6.6;
        public double BodyWidth//E
        {
            get { return bodyWidth; }
            set
            {
                bodyWidth = value;
                OnPropertyChanged(nameof(BodyWidth));
            }
        }

        double bodyHeight = 6.1;
        public double BodyHeight//D
        {
            get { return bodyHeight; }
            set
            {
                bodyHeight = value;
                OnPropertyChanged(nameof(BodyHeight));
            }
        }

        double bodyExtrusion = 2.2;
        public double BodyExtrusion//A
        {
            get { return bodyExtrusion; }
            set
            {
                bodyExtrusion = value;
                OnPropertyChanged(nameof(BodyExtrusion));
            }
        }

        double padWidth = 4.3;
        public double PadWidth//E1
        {
            get { return padWidth; }
            set
            {
                padWidth = value;
                OnPropertyChanged(nameof(PadWidth));
            }
        }

        double padHeight = 5.2;
        public double PadHeight//D1
        {
            get { return padHeight; }
            set
            {
                padHeight = value;
                OnPropertyChanged(nameof(PadHeight));
            }
        }

        double padExtrusion = 0.5;
        public double PadExtrusion//c1
        {
            get { return padExtrusion; }
            set
            {
                padExtrusion = value;
                OnPropertyChanged(nameof(PadExtrusion));
            }
        }

        double padOuterOffset = 1.2;
        public double PadOuterOffset//L3
        {
            get { return padOuterOffset; }
            set
            {
                padOuterOffset = value;
                OnPropertyChanged(nameof(PadOuterOffset));
            }
        }

        double pinWidth = 0.8;
        public double PinWidth //b
        {
            get { return pinWidth; }
            set
            {
                pinWidth = value;
                OnPropertyChanged(nameof(PinWidth));
            }
        }

        double pinThickness = 0.5;
        public double PinThickness //c
        {
            get { return pinThickness; }
            set
            {
                pinThickness = value;
                OnPropertyChanged(nameof(PinThickness));
            }
        }

        double pinPitch = 2.3;
        public double PinPitch //e
        {
            get { return pinPitch; }
            set
            {
                pinPitch = value;
                OnPropertyChanged(nameof(PinPitch));
            }
        }

        double pinBodyExitLength = 1.5;
        /// <summary>
        /// The length of the pin that exits the body before first bending
        /// </summary>
        public double PinBodyExitLength //L2
        {
            get { return pinBodyExitLength; }
            set
            {
                pinBodyExitLength = value;
                OnPropertyChanged(nameof(PinBodyExitLength));
            }
        }

        double pinLengthTotal = 2.75;
        /// <summary>
        /// The total length of the pin measured on horizontal
        /// </summary>
        public double PinLengthTotal //L1
        {
            get { return pinLengthTotal; }
            set
            {
                pinLengthTotal = value;
                OnPropertyChanged(nameof(PinLengthTotal));
            }
        }

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                //tab
                var padOffset = -(0.5 * (bodyHeight - padHeight) + padOuterOffset);
                meshItems.Add(CreateBody(new XPoint(0, padOffset), padWidth, padHeight, 0.5 * padOuterOffset, XColors.Silver, padExtrusion, 0.1 - 0.01));

                //body
                meshItems.Add(CreateBody(new XPoint(0, 0), bodyWidth, bodyHeight, 0.5 * (bodyWidth - padWidth), XColors.Black, bodyExtrusion));

                //pads
                var padZ = 0.5 * pinThickness;
                var padY = 0.5 * bodyHeight;
                meshItems.Add(CreatePad(new XPoint3D(-pinPitch, padY, padZ), 90, 1, pinWidth, pinThickness));
                meshItems.Add(CreatePad(new XPoint3D(pinPitch, padY, padZ), 90, 2, pinWidth, pinThickness));

                //middle pad
                meshItems.Add(new BoxMeshItem
                {
                    Y = padY,
                    Z = 0.5 * bodyExtrusion + 0.5 * pinThickness,
                    Width = pinWidth,
                    Height = pinThickness,
                    Length = 0.7 * pinBodyExitLength,
                    CanEdit = false,
                    IsPlaced = false,
                    FillColor = XColors.Silver
                });

                return meshItems;
            });
        }

        private IList<XPoint> GetBodyOutline(XPoint center, double width, double height, double cut)
        {
            //y positive down
            var points = new List<XPoint>()
            {
                new XPoint(-0.5 * width, -0.5 * height + cut),
                new XPoint(-0.5 * width + cut,  -0.5 * height),
                new XPoint( 0.5 * width - cut,  -0.5 * height),
                new XPoint( 0.5 * width,- 0.5 * height + cut),
                new XPoint(0.5 * width, 0.5 * height),
                new XPoint(-0.5 * width, 0.5 * height),
                new XPoint(-0.5 * width, -0.5 * height + cut)
            };

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                p.Offset(center.X, center.Y);
                points[i] = p;
            }

            return points;
        }

        private BaseMeshItem CreateBody(XPoint center, double width, double height, double cut, XColor bodyColor, double bodyExtrusion, double z = 0.1)
        {
            var outline = GetBodyOutline(center, width, height, cut);

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutline(outline, -bodyExtrusion));

            return new SolidBodyMeshItem
            {
                Model = mesh,
                FillColor = bodyColor,
                Z = z,
                IsPlaced = false,
                CanEdit = false
            };
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
            var pathHeight = 0.5 * pinThickness + 0.5 * bodyExtrusion;
            double upperSegmentLength = pinBodyExitLength;

            var bendAngleRadians = 0.25 * Math.PI;
            var dx = (pathHeight - 2 * bendRadius) / Math.Tan(bendAngleRadians);
            var lowerSegmentLength = pinLengthTotal - pinBodyExitLength - 0.5 * dx - 2 * bendRadius;

            var path = GeometryTools.CreateGullWingPathPin(pathHeight, upperSegmentLength, lowerSegmentLength, bendRadius, bendAngleRadians);

            return path;
        }

    }
}
