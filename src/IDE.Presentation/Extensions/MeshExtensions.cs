using HelixToolkit.Wpf.SharpDX;
using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Meshes;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Linq;

namespace IDE.Presentation.Extensions
{
    public static class MeshExtensions
    {



        public static IMeshModel ToMeshModel(this MeshGeometry3D geometry)
        {
            var model = new MeshModel();
            var mesh = geometry.ToMesh();
            mesh.Model = model;

            model.Meshes.Add(mesh);
            return model;
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
            if (geometry.Tangents != null)
            {
                mesh.Tangents = geometry.Tangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
            }
            if (geometry.BiTangents != null)
            {
                mesh.BiTangents = geometry.BiTangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
            }

            return mesh;
        }

       

       
    }
}
