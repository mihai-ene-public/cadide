using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Presentation.Meshes
{
    public class MeshModel : IMeshModel
    {
        public IList<IMesh> Meshes { get; set; } = new List<IMesh>();
        public XColor Color { get; set; } = XColors.White;
    }
}
