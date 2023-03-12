using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Coordinates;

/// <summary>
/// Canvas coordinate system
/// <para>Left - grows to the right</para>
/// <para>Top - grows downward</para>
/// <para>Origin is at (0, 0); will not be changed</para>
/// <para>This is an absolute coordinate system</para>
/// <para>Used for drawing stuff on canvas in WPF</para>
/// </summary>
public class TopLeftCoordinatesSystem : AbstractCoordinateSystem
{
    public TopLeftCoordinatesSystem()
    {
        Origin = new XPoint();
    }

    public override XPoint ConvertValueFrom(XPoint value, AbstractCoordinateSystem other)
    {
        if (other is TopLeftCoordinatesSystem)
        {
            return value;
        }
        else if (other is CartezianCoordinatesSystem)
        {
            var left = value.X + other.Origin.X;
            var top = other.Origin.Y - value.Y;
            return new XPoint(left, top);
        }

        return value;
    }
}
