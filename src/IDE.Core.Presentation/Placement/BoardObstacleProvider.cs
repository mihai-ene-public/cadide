using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.PlacementRouting;
using IDE.Core.Spatial2D;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Presentation.Placement
{
    public class BoardObstacleProvider : SimpleObstaclesProvider
    {
        Dictionary<ILayerDesignerItem, RTree<ObstacleItem>> trees;


        public BoardObstacleProvider(IDrawingViewModel drawingViewModel)
            : base(drawingViewModel)
        {

        }



        public void BuildObstacles()
        {
            trees = new Dictionary<ILayerDesignerItem, RTree<ObstacleItem>>();

            var brd = canvasModel.FileDocument as ILayeredViewModel;
            if (brd == null)
                return;

            var eligibleLayers = new[] { LayerType.Signal, LayerType.Plane };
            var layers = brd.LayerItems.Where(l => eligibleLayers.Contains(l.LayerType));

            foreach (var layer in layers)
            {
                trees[layer] = BuildObstaclesForLayer(layer);
            }

        }

        RTree<ObstacleItem> BuildObstaclesForLayer(ILayerDesignerItem layer)
        {
            var tree = new RTree<ObstacleItem>();
            var obstacles = new List<ObstacleItem>();



            var isTopLayer = layer.LayerId == LayerConstants.SignalTopLayerId;
            var isBottomLayer = layer.LayerId == LayerConstants.SignalBottomLayerId;
            var placedItems = canvasModel.GetItems().Cast<BaseCanvasItem>().Where(c => c.IsPlaced).ToList();

            List<HoleCanvasItem> drillHoles = new List<HoleCanvasItem>();
            var vias = new List<ViaCanvasItem>();

            var footprintItems = (from fp in placedItems.OfType<FootprintBoardCanvasItem>()
                                  from p in fp.Items
                                  select (BaseCanvasItem)((p as BaseCanvasItem).Clone())).ToList();

            if (isTopLayer || isBottomLayer)
            {
                obstacles.AddRange(footprintItems.OfType<SingleLayerBoardCanvasItem>()
                                                 .Where(c => c.Layer != null && c.Layer.LayerId == layer.LayerId)
                                                 .Select(p => new ObstacleItem
                                                 {
                                                     CanvasItem = p
                                                 }));
                obstacles.AddRange(footprintItems.OfType<IPadCanvasItem>().Cast<BaseCanvasItem>()
                                                 .Select(p => new ObstacleItem
                                                 {
                                                     CanvasItem = p
                                                 }));
            }

            //drill holes go thru all layers
            //holes from footprints
            drillHoles.AddRange(footprintItems.OfType<HoleCanvasItem>());
            drillHoles.AddRange(placedItems.OfType<HoleCanvasItem>());

            //vias; needs to be filtered for layer (for now they are thru via)
            vias.AddRange(placedItems.OfType<ViaCanvasItem>());

            //milling items - all layers
            var millingItems = placedItems.OfType<SingleLayerBoardCanvasItem>()
                                               .Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId)
                                               .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId))
                                               .OfType<BaseCanvasItem>().ToList();

            obstacles.AddRange(drillHoles.Select(p => new ObstacleItem { CanvasItem = p }));
            obstacles.AddRange(millingItems.Select(p => new ObstacleItem { CanvasItem = p }));

            obstacles.AddRange(vias.Select(p => new ObstacleItem { CanvasItem = p }));

            //things from the layer
            var layerItems = placedItems.OfType<SingleLayerBoardCanvasItem>()
                                        .Where(l => l.Layer != null && l.Layer == layer)
                                        .ToList();

            obstacles.AddRange(layerItems.Select(p => new ObstacleItem { CanvasItem = p }));

            foreach (var obstacle in obstacles)
            {
                var t = obstacle.CanvasItem.GetTransform();

                var geometry = GeometryHelper.GetGeometry(obstacle.CanvasItem, applyTransform: true);
                //if (!geometry.IsEmpty())
                //    geometry.Transform = t.ToMatrixTransform();
                obstacle.Geometry = geometry;

                var bounds = GeometryHelper.GetGeometryBounds(geometry);
                obstacle.Envelope = Envelope.FromRect(bounds);

                //tree.Add(Core.RTree.Rectangle.FromRect(bounds), obstacle);
                tree.Insert(obstacle);
            }

            return tree;
        }

        ILayerDesignerItem GetCurrentLayer()
        {
            var brd = canvasModel.FileDocument as ILayeredViewModel;
            return brd?.SelectedLayer;
        }

        protected override RTree<ObstacleItem> GetTree()
        {
            var layer = GetCurrentLayer();
            if (trees.ContainsKey(layer))
                return trees[layer];

            return null;
        }



        public IList<ObstacleItem> GetNearestObstaclesAllLayers(XPoint point, double maxDistance)
        {
            var output = new List<ObstacleItem>();
            foreach (var tree in trees.Values)
            {
                if (tree != null)
                    output.AddRange(tree.SearchNearest(point, maxDistance));
            }


            return output;
        }


    }
}
