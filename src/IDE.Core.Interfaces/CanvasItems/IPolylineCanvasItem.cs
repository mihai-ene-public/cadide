using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Interfaces
{
    public interface IPolylineCanvasItem : ISelectableItem
    {
        IList<XPoint> Points { get; set; }

        double Width { get; set; }

        void OnPropertyChanged(string propertyName);
    }
}
