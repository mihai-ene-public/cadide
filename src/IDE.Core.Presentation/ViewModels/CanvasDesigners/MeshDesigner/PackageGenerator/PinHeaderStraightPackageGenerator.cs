using IDE.Core.Common.Geometries;
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
    public class PinHeaderStraightPackageGenerator : PackageGenerator
    {
        public override string Name => "Pin Header - Straight";

        int numberRows = 1;

        public int NumberRows
        {
            get { return numberRows; }
            set
            {
                numberRows = value;
                OnPropertyChanged(nameof(NumberRows));
            }
        }

        int numberColumns = 5;

        public int NumberColumns
        {
            get { return numberColumns; }
            set
            {
                numberColumns = value;
                OnPropertyChanged(nameof(NumberColumns));
            }
        }

        double e = 2.54;

        /// <summary>
        /// E - width
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

        double d = 12.70d;
        /// <summary>
        /// D - body height
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

        double pinPitchE = 2.54d;
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchE
        {
            get { return pinPitchE; }
            set
            {
                pinPitchE = value;
                OnPropertyChanged(nameof(PinPitchE));
            }
        }

        double pinPitchD = 2.54d;
        /// <summary>
        /// D - body height
        /// </summary>
        public double PinPitchD
        {
            get { return pinPitchD; }
            set
            {
                pinPitchD = value;
                OnPropertyChanged(nameof(PinPitchD));
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

        double l = 2.54d;

        public double L
        {
            get { return l; }
            set
            {
                l = value;
                OnPropertyChanged(nameof(L));
            }
        }

        double l1 = 3d;

        public double L1
        {
            get { return l1; }
            set
            {
                l1 = value;
                OnPropertyChanged(nameof(L1));
            }
        }


        double l2 = 5.84d;

        public double L2
        {
            get { return l2; }
            set
            {
                l2 = value;
                OnPropertyChanged(nameof(L2));
            }
        }

        bool isFemale = false;

        public bool IsFemale
        {
            get { return isFemale; }
            set
            {
                isFemale = value;
                OnPropertyChanged(nameof(IsFemale));
            }
        }

        public override Task<List<BaseMeshItem>> GeneratePackage()
        {
            return Task.Run(() =>
            {
                var meshItems = new List<BaseMeshItem>();

                meshItems.Add(CreateBody());

                CreatePads(meshItems);

                return meshItems;
            });
        }

        private void CreatePads(List<BaseMeshItem> meshItems)
        {
            var rowOffset = 0.5 * (d - (numberColumns - 1) * pinPitchD);
            var colOffset = 0.5 * (e - (numberRows - 1) * pinPitchE);

            var pinHeight = isFemale ? l1 : l1 + l2 + l;
            var z = 0.5 * pinHeight - l1;//for male

            //pins
            for (int rowIndex = 0; rowIndex < NumberRows; rowIndex++)
            {
                var y = -0.5 * e + colOffset + rowIndex * pinPitchE;
                for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
                {
                    var x = -0.5 * d + rowOffset + colIndex * pinPitchD;

                    meshItems.Add(new BoxMeshItem
                    {
                        X = x,
                        Y = y,
                        Z = z + 0.01,
                        Width = b,
                        Length = b,
                        Height = pinHeight,
                        FillColor = XColors.Silver,
                        IsPlaced = false,
                        CanEdit = false,

                    });
                }
            }
        }

        BaseMeshItem CreateBody()
        {
            if (isFemale)
            {
                return CreateFemaleBody();
            }
            else
            {
                return CreateMaleBody();
            }
        }

        BaseMeshItem CreateMaleBody()
        {
            var points = new List<XPoint>();

            var rowOffset = 0.5 * (d - (numberColumns - 1) * pinPitchD);
            var colOffset = 0.5 * (e - (numberRows - 1) * pinPitchE);

            var splitWidth = 1.0d;
            var splitDepth = 0.5d;
            //upper part
            // /-
            for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
            {
                //var y = -0.5 * e + splitDepth;
                var x = -0.5 * d + colIndex * pinPitchD;

                points.Add(new XPoint(x, -0.5 * e + splitDepth));
                points.Add(new XPoint(x + 0.5 * splitWidth, -0.5 * e));
                points.Add(new XPoint(x + PinPitchD - 0.5 * splitWidth, -0.5 * e));
            }

            // add last upper part point
            points.Add(new XPoint(0.5 * d, -0.5 * e + splitDepth));
            points.Add(new XPoint(0.5 * d, 0.5 * e - splitDepth));

            //lower part
            for (int colIndex = numberColumns - 1; colIndex >= 0; colIndex--)
            {
                //var y = 0.5 * e - splitDepth;
                var x = -0.5 * d + colIndex * pinPitchD;

                points.Add(new XPoint(x + PinPitchD - 0.5 * splitWidth, 0.5 * e));
                points.Add(new XPoint(x + 0.5 * splitWidth, 0.5 * e));
                points.Add(new XPoint(x, 0.5 * e - splitDepth));
            }

            //add first point to close outline
            points.Add(points[0]);

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutline(points, -l));

            return new SolidBodyMeshItem
            {
                Model = mesh,
                FillColor = XColors.Black,
                IsPlaced = false,
                CanEdit = false
            };
        }

        BaseMeshItem CreateFemaleBody()
        {
            var g = new GroupMeshItem();
            g.Items.Add(new BoxMeshItem
            {
                BottomFace = false,
                Z = 0.5 * l,
                Width = d,
                Height = l,
                Length = e,
                CanEdit = false,
                IsPlaced = false,
                RotationZ = 90,
                FillColor = XColors.Black
            });

            IList<IList<XPoint>> fills = new List<IList<XPoint>>();
            IList<IList<XPoint>> holes = new List<IList<XPoint>>();
            IList<IList<XPoint>> pinHoles = new List<IList<XPoint>>();

            var rectOutline = new RectangleGeometryOutline(0, 0, d, e);

            fills.Add(rectOutline.GetOutline());

            //create upper face
            var rowOffset = 0.5 * (d - (numberColumns - 1) * pinPitchD);
            var colOffset = 0.5 * (e - (numberRows - 1) * pinPitchE);

            var pinHeight = isFemale ? l1 : l1 + l2 + l;
            var z = 0.5 * pinHeight - l1;//for male

            var pinClearance = b + 1.0d;
            //foreach pin add the hole
            for (int rowIndex = 0; rowIndex < NumberRows; rowIndex++)
            {
                var y = -0.5 * e + colOffset + rowIndex * pinPitchE;
                for (int colIndex = 0; colIndex < NumberColumns; colIndex++)
                {
                    var x = -0.5 * d + rowOffset + colIndex * pinPitchD;

                    var holeOutline = new RectangleGeometryOutline(x, y, pinClearance, pinClearance).GetOutline();
                    holes.Add(holeOutline);
                    var pinOutline = new RectangleGeometryOutline(x, y, b, b).GetOutline();
                    pinHoles.Add(pinOutline);

                    IList<IList<XPoint3D>> loftedPoints = new List<IList<XPoint3D>>();

                    //upper
                    loftedPoints.Add(holeOutline.Select(p => new XPoint3D(p.X, p.Y, 0)).ToList());

                    //lower
                    loftedPoints.Add(pinOutline.Select(p => new XPoint3D(p.X, p.Y, 0.5d)).ToList());

                    IMeshModel loftedMesh = null;
                    dispatcher.RunOnDispatcher(() => loftedMesh = meshHelper.CreateLoftedGeometry(loftedPoints));

                    g.Items.Add(new SolidBodyMeshItem
                    {
                        Z = l,
                        Model = loftedMesh,
                        FillColor = XColors.Black,
                        IsPlaced = false,
                        CanEdit = false
                    });

                    g.Items.Add(new BoxMeshItem
                    {
                        X = x,
                        Y = y,
                        Z = 0.75 * l - 0.5d,
                        BottomFace = false,
                        Width = b,
                        Length = b,
                        Height = 0.5 * l,
                        CanEdit = false,
                        IsPlaced = false,
                        FillColor = XColors.Black
                    });

                }
            }

            IMeshModel mesh = null;
            dispatcher.RunOnDispatcher(() => mesh = meshHelper.ExtrudeOutlines(fills, holes, 0));

            g.Items.Add(new SolidBodyMeshItem
            {
                Z = l,
                Model = mesh,
                FillColor = XColors.Black,
                IsPlaced = false,
                CanEdit = false
            });

            return g;
        }
    }

}
