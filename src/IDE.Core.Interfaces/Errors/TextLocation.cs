using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Windows;

namespace IDE.Core.Errors
{
    public class TextLocation : IErrorLocation
    {
        public IFileBaseViewModel File { get; set; }
        public XPoint? Location { get; set; }
    }


}
