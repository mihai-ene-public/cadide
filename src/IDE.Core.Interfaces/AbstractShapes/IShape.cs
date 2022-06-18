using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// a generic shape regardless of where it is presented
    /// </summary>
    public interface IShape
    {
    }

    public interface IArcShape : IShape
    {
        XPoint StartPoint { get; set; }

        XPoint EndPoint { get; set; }

        //stroke width
        double Width { get; set; }

        XSweepDirection SweepDirection { get; set; }

        double SizeDiameter { get; set; }
    }

    public interface ILineShape : IShape
    {
        XPoint StartPoint { get; set; }
        XPoint EndPoint { get; set; }
        double Width { get; set; }
    }

    public interface ICircleShape : IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double Diameter { get; set; }

        bool IsFilled { get; set; }
        double BorderWidth { get; set; }
    }

    public interface IFigureShape : IShape
    {
        IList<IShape> FigureShapes { get; }
    }

    public interface IHoleShape : IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double Drill { get; set; }
    }

    public interface IPolygonShape : IShape
    {
        IList<XPoint> Points { get; set; }
    }

    public interface IPolylineShape : IShape
    {
        double Width { get; set; }

        IList<XPoint> Points { get; set; }
    }

    public interface IPouredPolygonShape : IShape
    {
        IGeometryOutline FinalGeometry { get; set; }
    }

    public interface IRectangleShape : IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double CornerRadius { get; set; }

        double Rot { get; set; }

        double BorderWidth { get; set; }

        bool IsFilled { get; set; }
    }

    public interface ITextShape : IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double Rot { get; set; }
        double FontSize { get; set; }
        double Width { get; set; }
        string FontFamily { get; set; }
        string Text { get; set; }
        bool Italic { get; set; }
        bool Bold { get; set; }
    }

    public interface IViaShape : IShape
    {
        double X { get; set; }
        double Y { get; set; }
        double Drill { get; set; }
        double PadDiameter { get; set; }
    }

    public interface IRegionShape : IShape
    {
        XPoint StartPoint { get; }
        double Width { get; }

        IList<IShape> Items { get; set; }
    }
}
