using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Coordinates;


/// <summary>
/// an abstract basic 2D coordinate system
/// </summary>
public abstract class AbstractCoordinateSystem// : BaseViewModel
{
    XPoint origin;
    public XPoint Origin
    {
        get
        {
            return origin;
        }
        set
        {
            origin = value;
            //OnPropertyChanged(nameof(Origin));
        }
    }


    public abstract XPoint ConvertValueFrom(XPoint value, AbstractCoordinateSystem other);

    public double ConvertValueFrom(double value, Axis axis, AbstractCoordinateSystem other)
    {
        var p = new XPoint(value, value);
        var cp = ConvertValueFrom(p, other);
        return axis == Axis.X ? cp.X : cp.Y;
    }

}
