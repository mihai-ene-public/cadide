using IDE.Core.Types.Media3D;
using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces
{
    public interface IMesh
    {
        int[] Indices { get; set; }
        XPoint3D[] Positions { get; set; }

        XVector3D[] Normals { get; set; }
        XVector3D[] Tangents { get; set; }
        XVector3D[] BiTangents { get; set; }

        XPoint[] TextureCoordinates { get; set; }

        XColor Color { get; set; }
        IMeshModel Model { get; set; }
    }
}