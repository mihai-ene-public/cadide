using HelixToolkit.Wpf;
using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Meshes;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace IDE.Presentation.Extensions
{
    public static class MeshWpfExtensions
    {
        public static BaseMeshItem ToMeshItem(this Model3DGroup model3DGroup)
        {
            var items = new List<BaseMeshItem>();

            if (model3DGroup != null)
            {
                foreach (var geometryModel in model3DGroup.Children.OfType<GeometryModel3D>())
                {
                    var geometry = geometryModel.Geometry as MeshGeometry3D;
                    if (geometry == null)
                        continue;

                    var solid = geometry.ToSolidBodyMeshItem();
                    solid.FillColor = GetColorFromMaterial(geometryModel.Material);

                    items.Add(solid);
                }
            }

            foreach (var item in items)
                item.IsPlaced = true;

            if (items.Count > 1)
            {
                var g = new GroupMeshItem();
                g.IsPlaced = true;
                g.Items.AddRange(items);
                return g;
            }

            return items.FirstOrDefault();
        }

        public static SolidBodyMeshItem ToSolidBodyMeshItem(this MeshGeometry3D geometry)
        {
            var meshModel = new MeshModel();
            var mesh = geometry.ToMesh();
            mesh.Model = meshModel;
            meshModel.Meshes.Add(mesh);

            var solid = new SolidBodyMeshItem
            {
                Model = meshModel,
            };

            return solid;
        }

        private static XColor GetColorFromMaterial(Material geometryMaterial)
        {
            var color = XColors.White;
            var material = MaterialHelper.GetFirst<DiffuseMaterial>(geometryMaterial);
            if (material != null)
            {
                color = material.Color.ToXColor();
            }

            return color;
        }

      

        public static IMeshModel ToMeshModel(this Model3DGroup model3DGroup)
        {
            var meshModel = new MeshModel();

            foreach (var geometryModel in model3DGroup.Children.OfType<GeometryModel3D>())
            {
                var geometry = geometryModel.Geometry as MeshGeometry3D;
                if (geometry == null)
                    continue;

                var color = GetColorFromMaterial(geometryModel.Material);

                var mesh = geometry.ToMesh();
                mesh.Model = meshModel;
                mesh.Color = color;

                meshModel.Meshes.Add(mesh);
            }

            return meshModel;
        }

        public static IMesh ToMesh(this MeshGeometry3D geometry)
        {
            var mesh = new Mesh
            {
                Positions = geometry.Positions.Select(p => new XPoint3D(p.X, p.Y, p.Z)).ToArray(),
                Indices = geometry.TriangleIndices.ToArray(),
            };

            if (geometry.Normals != null)
            {
                mesh.Normals = geometry.Normals.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
            }
            if (geometry.TextureCoordinates != null)
            {
                mesh.TextureCoordinates = geometry.TextureCoordinates.Select(p => new XPoint(p.X, p.Y)).ToArray();
            }
            //if (geometry..Tangents != null)
            //{
            //    mesh.Tangents = geometry.Tangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
            //}
            //if (geometry.BiTangents != null)
            //{
            //    mesh.BiTangents = geometry.BiTangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
            //}

            return mesh;
        }

        public static Model3DGroup ToModel3DGroup(this IMeshModel meshModel)
        {
            var mg = new Model3DGroup();

            foreach (var mesh in meshModel.Meshes)
            {
                var gm = new GeometryModel3D();
                var meshGeom = mesh.ToMeshGeometry3D();
                gm.Geometry = meshGeom;

                var material = MaterialHelper.CreateMaterial(mesh.Color.ToColor());
                gm.Material = material;
                gm.BackMaterial = material;

                mg.Children.Add(gm);
            }

            return mg;
        }

        public static MeshGeometry3D ToMeshGeometry3D(this IMesh geometry)
        {
            var mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection(geometry.Positions.Select(p => new Point3D(p.X, p.Y, p.Z))),
                TriangleIndices = new System.Windows.Media.Int32Collection(geometry.Indices),
            };

            if (geometry.Normals != null)
            {
                mesh.Normals = new Vector3DCollection(geometry.Normals.Select(p => new Vector3D(p.X, p.Y, p.Z)));
            }
            if (geometry.TextureCoordinates != null)
            {
                mesh.TextureCoordinates = new System.Windows.Media.PointCollection(geometry.TextureCoordinates.Select(p => new System.Windows.Point(p.X, p.Y)));
            }

            return mesh;
        }

        public static Vector3D FindAnyPerpendicular(this Vector3D n)
        {
            n.Normalize();
            Vector3D u = Vector3D.CrossProduct(new Vector3D(0, 1, 0), n);
            if (u.LengthSquared < 1e-3)
            {
                u = Vector3D.CrossProduct(new Vector3D(1, 0, 0), n);
            }

            return u;
        }
    }

}
