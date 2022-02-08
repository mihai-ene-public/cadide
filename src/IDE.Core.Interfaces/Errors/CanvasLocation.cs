using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Windows;


namespace IDE.Core.Errors
{
    public class CanvasLocation : IErrorLocation
    {
        public IFileBaseViewModel File { get; set; }
        public XRect? Location { get; set; }

        //?
        public IGeometry Geometry { get; set; }
    }


}
