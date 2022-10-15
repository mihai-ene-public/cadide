using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using System.Windows;


namespace IDE.Core.Errors
{
    public class CanvasLocation : IErrorLocation
    {
        public IFileBaseViewModel File { get; set; }
        public XRect? Location { get; set; }

        public IGeometryOutline Geometry { get; set; }
    }


}
