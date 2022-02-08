using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Coordinates
{

    /// <summary>
    /// X, Y coordinate system
    /// <para>X grows to right</para>
    /// <para>Y grows upwards</para>
    /// <para>Origin is at any arbitrary point relative to Top, Left in Canvas coordinates system</para>
    /// </summary>
    public class CartezianCoordinatesSystem : AbstractCoordinateSystem
    {

        public CartezianCoordinatesSystem()
        {
            const double defaultOriginTop = 50;
            const double defaultOriginLeft = 50;
            Origin = new XPoint(defaultOriginLeft, defaultOriginTop);
        }

        public override XPoint ConvertValueFrom(XPoint value, AbstractCoordinateSystem other)
        {
            if (other is TopLeftCoordinatesSystem)
            {
                var x = value.X - Origin.X;
                var y = Origin.Y - value.Y;
                return new XPoint(x, y);
            }
            else if (other is CartezianCoordinatesSystem)
            {
                //value.Offset(-Origin.X, -Origin.Y);
                return value;

            }
            return value;
        }
    }

    public class CoordinateSystems
    {
        static CartezianCoordinatesSystem cartezian;
        

        public static CartezianCoordinatesSystem Cartezian
        {
            get
            {
                if (cartezian == null)
                    cartezian = new CartezianCoordinatesSystem();
                return cartezian;
            }
        }

        static TopLeftCoordinatesSystem topLeft;
        public static TopLeftCoordinatesSystem TopLeft
        {
            get
            {
                if (topLeft == null)
                    topLeft = new TopLeftCoordinatesSystem();
                return topLeft;
            }
        }
    }
}
