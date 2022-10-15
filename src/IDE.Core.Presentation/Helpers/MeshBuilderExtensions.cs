using IDE.Core.Interfaces;
using IDE.Core.Presentation.Meshes;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using IDE.Core.Types.MeshBuilding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.Presentation.Extensions;

public static class MeshBuilderExtensions
{
    public static IMeshModel ToMeshModel(this XMeshBuilder meshBuilder, XColor color)
    {
        var meshModel = new MeshModel { Color = color };
        var mesh = new Mesh
        {
            Color = color,
            Positions = meshBuilder.Positions.Select(p => new XPoint3D(p.X, p.Y, p.Z)).ToArray(),
            Indices = meshBuilder.TriangleIndices.ToArray(),
            Model = meshModel
        };

        if (meshBuilder.Normals != null)
        {
            mesh.Normals = meshBuilder.Normals.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
        }
        if (meshBuilder.TextureCoordinates != null)
        {
            mesh.TextureCoordinates = meshBuilder.TextureCoordinates.Select(p => new XPoint(p.X, p.Y)).ToArray();
        }
        if (meshBuilder.Tangents != null)
        {
            mesh.Tangents = meshBuilder.Tangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
        }
        if (meshBuilder.BiTangents != null)
        {
            mesh.BiTangents = meshBuilder.BiTangents.Select(p => new XVector3D(p.X, p.Y, p.Z)).ToArray();
        }

        meshModel.Meshes.Add(mesh);

        return meshModel;
    }
}
