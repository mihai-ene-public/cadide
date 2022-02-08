using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDE.Core.Interfaces
{
    public interface ITrackBoardCanvasItem : ISignalPrimitiveCanvasItem
                                           , ISegmentedPolylineSelectableCanvasItem
    {
    }
}
