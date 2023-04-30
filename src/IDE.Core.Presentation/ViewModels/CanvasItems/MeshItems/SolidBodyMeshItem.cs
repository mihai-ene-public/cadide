using IDE.Core.Storage;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using IDE.Core.Types.Media;
using System.Xml;
using System.IO;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media3D;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Presentation.Meshes;

namespace IDE.Core.Designers
{
    public class SolidBodyMeshItem : BaseMeshItem
    {
        public SolidBodyMeshItem()
        {
            FillColor = XColors.WhiteSmoke;

            PropertyChanged += SolidBodyMeshItem_PropertyChanged;
        }

        private void SolidBodyMeshItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(FillColor):
                    ApplyModelColor();
                    break;
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 5)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Center));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 6)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Center));
            }
        }

        double z;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 7)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged(nameof(Z));
                OnPropertyChanged(nameof(Center));
            }
        }

        [Browsable(false)]
        public XPoint3D Center
        {
            get
            {
                return new XPoint3D(X, Y, -Z);
            }
        }



        double rotationX;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 8)]
        [MarksDirty]
        public double RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value;
                OnPropertyChanged(nameof(RotationX));
            }
        }

        double rotationY;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 9)]
        [MarksDirty]
        public double RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value;
                OnPropertyChanged(nameof(RotationY));
            }
        }

        double rotationZ;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 10)]
        [MarksDirty]
        public double RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value;
                OnPropertyChanged(nameof(RotationZ));
            }
        }

        IMeshModel model;

        [Browsable(false)]
        public IMeshModel Model
        {
            get { return model; }
            set
            {
                model = value;
                if (model == null)
                    return;

                ApplyModelColor();

                OnPropertyChanged(nameof(Model));
            }
        }

        void ApplyModelColor()
        {
            if (model == null)
                return;

            model.Color = FillColor;
            foreach (var mesh in model.Meshes)
                mesh.Color = FillColor;

            OnPropertyChanged(nameof(Model));
        }

        public override void Translate(double dx, double dy, double dz)
        {
            X += dx;
            Y += dy;
            Z += dz;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (Mesh3DItem)primitive;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
            PadNumber = r.PadNumber;
            FillColor = XColor.FromHexString(r.FillColor);

            //var meshHelper = ServiceProvider.Resolve<IMeshHelper>();
            //Model = meshHelper.GetMeshModelFromCData(r.ModelCData);

            var model = new MeshModel();
            var mesh = new Mesh()
            {
                Model = model,
                Color = FillColor,

            };
            if (r.MeshGeometry != null)
            {
                mesh.Positions = r.MeshGeometry.Positions;
                mesh.Normals = r.MeshGeometry.Normals;
                mesh.Indices = r.MeshGeometry.Indices;
                mesh.TextureCoordinates = r.MeshGeometry.TextureCoordinates;
            }
            model.Meshes.Add(mesh);

            Model = model;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new Mesh3DItem();

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.RotationX = RotationX;
            r.RotationY = RotationY;
            r.RotationZ = RotationZ;
            r.FillColor = FillColor.ToHexString();
            r.PadNumber = PadNumber;

            //var meshHelper = ServiceProvider.Resolve<IMeshHelper>();
            //r.ModelCData = meshHelper.GetCDataFromMeshModel(Model);
            var mesh = model.Meshes[0];
            r.MeshGeometry = new MeshGeometryData
            {
                Positions = mesh.Positions,
                Normals = mesh.Normals,
                Indices = mesh.Indices,
                TextureCoordinates = mesh.TextureCoordinates
            };

            return r;
        }

        public override void MirrorX()
        {
            var r = ( rotationX + 180 ) % 360;
            RotationX = r;
        }

        public override void MirrorY()
        {
            var r = ( rotationY + 180 ) % 360;
            RotationY = r;
        }

        public override void TransformBy(XMatrix3D matrix)
        {
            var p = new XPoint3D(x, y, z);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;
            Z = p.Z;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            RotationZ = RotateSafe(rotationZ, rotAngle);

        }

        public override void Rotate(double angle = 90)
        {
            // var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ, angle);
        }

        public override string ToString()
        {
            return $"Solid ({PadNumber})";
        }


    }

    public class SolidComponentMeshItem : BaseMeshItem
    {
        XColor fillColor = XColors.Gray;

        [Display(Order = 1)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public override XColor FillColor
        {
            get
            {
                return fillColor;
            }

            set
            {
                fillColor = value;
                //if (model is GeometryModel3D g)
                //{
                //var m=MaterialHelper.CreateMaterial(value);
                //g.Material = m;
                //g.BackMaterial = m;

                //}
                OnPropertyChanged(nameof(FillColor));
                OnPropertyChanged(nameof(PresentedFillColor));
                OnPropertyChanged(nameof(IsTransparent));
            }
        }

        /*Model3D*/
        object model;

        /// <summary>
        /// Model3D
        /// </summary>
        public object /*Model3D*/ Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        public override void MirrorX()
        {

        }

        public override void MirrorY()
        {

        }

        public override void Rotate(double angle = 90)
        {

        }

        public override void Translate(double dx, double dy, double dz)
        {

        }
    }
}
