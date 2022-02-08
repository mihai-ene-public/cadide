using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// represents a 3D model that contains other meshes
    /// <para>Each Mesh has its own material/color</para>
    /// </summary>
    public interface IMeshModel
    {
        IList<IMesh> Meshes { get; set; }

        XColor Color { get; set; }
    }
}