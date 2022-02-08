using IDE.Core.Types.Media;
using System;

namespace IDE.Core.Designers
{
    // Wraps info of the dragged object into a class
    public class DragObject
    {
        public XSize? DesiredSize { get; set; }
        public Type ContentType { get; set; }

    }
}
