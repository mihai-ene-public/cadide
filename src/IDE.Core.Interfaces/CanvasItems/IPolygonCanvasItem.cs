using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IDE.Core.Interfaces
{
    public interface IPolygonCanvasItem : ISelectableItem
    {
        void OnPropertyChanged(string propertyName);

        IList<XPoint> PolygonPoints { get; set; }

        double BorderWidth { get; set; }

        bool IsFilled { get; }

        PolygonType PolygonType { get; }





        //void RepourPolygon();
    }

    public interface IPolySchematicCanvasItem : IPolygonCanvasItem
    {
        XColor BorderColor { get; set; }

        XColor FillColor { get; set; }
    }

    public interface IPolygonBoardCanvasItem : IPolygonCanvasItem, IGenerateThermals, IExcludeItems, IRepourGeometry
    {
        IGeometry PolygonGeometry { get; }
    }

    public interface IGenerateThermals
    {
        bool GenerateThermals { get; set; }

        double ThermalWidth { get; set; }
    }

    public interface IExcludeItems
    {
        IList<IItemWithClearance> GetExcludedItems();
    }

    public interface IItemWithClearance
    {
        ISelectableItem CanvasItem { get; }

        double Clearance { get; }
    }

    public interface IPolygonGeometryPourProcessor
    {
        object GetGeometry(ISelectableItem thisItem, IBoardDesigner board);
    }

    public interface IPolygonGeometryOutlinePourProcessor
    {
        IGeometryOutline GetGeometry(ISelectableItem thisItem, IBoardDesigner board);
    }

    public interface IPlaneGeometryPourProcessor
    {
        object GetGeometry(ISelectableItem thisItem, IBoardDesigner board);
    }

    public enum PolygonType
    {
        Fill,
        Keepout
    }
}
