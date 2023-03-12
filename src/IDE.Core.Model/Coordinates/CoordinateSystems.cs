namespace IDE.Core.Coordinates;

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
