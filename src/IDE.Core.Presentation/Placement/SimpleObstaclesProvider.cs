using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Presentation.PlacementRouting;
using IDE.Core.Spatial2D;
using IDE.Core.Types.Media;
using System.Collections.Generic;

namespace IDE.Core.Presentation.Placement
{
    public class SimpleObstaclesProvider
    {
        public SimpleObstaclesProvider()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
        }

        public SimpleObstaclesProvider(IDrawingViewModel drawingViewModel) : this()
        {
            canvasModel = drawingViewModel;
        }

        protected IDrawingViewModel canvasModel;

        protected IGeometryOutlineHelper GeometryHelper;

        RTree<ObstacleItem> tree;

        public void BuildTree(IEnumerable<ISelectableItem> items)
        {
            tree = new RTree<ObstacleItem>();

            foreach (var item in items)
            {
                var t = item.GetTransform();

                var geometry = GeometryHelper.GetGeometry(item, applyTransform: true);

                var rect = geometry.GetBounds();
                rect = t.TransformBounds(rect);//?

                var obs = new ObstacleItem
                {
                    CanvasItem = item,
                    Geometry = geometry,
                    Envelope = Envelope.FromRect(rect)
                };


                //tree.Add(Core.RTree.Rectangle.FromRect(rect), obs);
                tree.Insert(obs);
            }
        }

        public IList<ObstacleItem> GetNearestObstacles(XPoint point, double maxDistance)
        {
            var tree = GetTree();
            if (tree != null)
                return tree.SearchNearest(point, maxDistance);

            return new List<ObstacleItem>();
        }

        public IList<ObstacleItem> GetObstaclesInRectangle(XRect rect)
        {
            var tree = GetTree();
            if (tree != null)
                return tree.Search(rect);

            return new List<ObstacleItem>();
        }

        protected virtual RTree<ObstacleItem> GetTree()
        {
            return tree;
        }
    }
}
