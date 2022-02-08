using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;

namespace IDE.Core.Presentation.Meshes
{
    public class Mesh : IMesh
    {
        public int[] Indices { get; set; }
        public XPoint3D[] Positions { get; set; }
        public XVector3D[] Normals { get; set; }
        public XPoint[] TextureCoordinates { get; set; }
        public XColor Color { get; set; } = XColors.White;
        public IMeshModel Model { get; set; }
        public XVector3D[] Tangents { get; set; }
        public XVector3D[] BiTangents { get; set; }
    }
}
