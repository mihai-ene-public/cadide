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

    public interface IPolygonBoardCanvasItem : IPolygonCanvasItem,
        ISignalPrimitiveCanvasItem,
        ISingleLayerBoardCanvasItem,
        IGenerateThermals,
        IExcludeItems,
        IRepourGeometry
    {
        IGeometry PolygonGeometry { get; }
        int DrawOrder { get; set; }
    }
    public interface ISingleLayerBoardCanvasItem : ISelectableItem
    {
        ILayerDesignerItem Layer { get; set; }

        bool ShouldBeOnLayer(ILayerDesignerItem layer);
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

    public class ItemWithClearance : IItemWithClearance
    {
        public ItemWithClearance(ISelectableItem canvasItem, double clearance)
        {
            CanvasItem = canvasItem;
            Clearance = clearance;
        }

        public ISelectableItem CanvasItem { get; private set; }

        public double Clearance { get; private set; }
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
