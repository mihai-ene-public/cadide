using HelixToolkit.Wpf.SharpDX;
using IDE.Core;
using IDE.Core.Interfaces;
using SharpDX;
using System.Linq;

namespace IDE.Presentation.Extensions
{
    public static class SharpDXMeshExtensions
    {
        public static MeshGeometry3D ToSharpDXMeshGeometry3D(this IMesh geometry)
        {
            var mesh = new MeshGeometry3D
            {
                Positions = new Vector3Collection(geometry.Positions.Select(p => new Vector3((float)p.X, (float)p.Y, (float)p.Z))),
                TriangleIndices = new IntCollection(geometry.Indices),
            };

            if (geometry.Normals != null)
            {
                mesh.Normals = new Vector3Collection(geometry.Normals.Select(p => new Vector3((float)p.X, (float)p.Y, (float)p.Z)));
            }
            if (geometry.TextureCoordinates != null)
            {
                mesh.TextureCoordinates = new Vector2Collection(geometry.TextureCoordinates.Select(p => new Vector2((float)p.X, (float)p.Y)));
            }
            if(geometry.Tangents!=null)
            {
                mesh.Tangents = new Vector3Collection(geometry.Tangents.Select(p => p.ToVector3()));
            }
            if (geometry.BiTangents != null)
            {
                mesh.BiTangents = new Vector3Collection(geometry.BiTangents.Select(p => p.ToVector3()));
            }
            return mesh;
        }
    }
}
