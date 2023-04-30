using Eagle;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Documents.Views;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class AssemblyPickAndPlaceHelper
    {
        private List<AssemblyPickAndPlaceItemDisplay> GetPickAndPlaceList(IBoardDesigner board)
        {
            var buildOptions = ( (BoardBuildOptionsViewModel)board.BuildOptions ).Assembly;

            var items = new List<AssemblyPickAndPlaceItemDisplay>();

            var boardRectangle = board.GetBoardRectangle();
            var boardOriginX = boardRectangle.BottomLeft.X;
            var boardOriginY = boardRectangle.BottomLeft.Y;
            var useImperial = buildOptions.PositionUnits == OutputUnits.inch;

            var parts = board.GetFootprints()
                                    .OrderBy(f => f.PartName, new IndexedNameComparer())
                                    .ToList();

            foreach (var p in parts)
            {
                var assemblyItem = new AssemblyPickAndPlaceItemDisplay
                {
                    PartName = p.PartName,
                    Layer = p.Placement.ToString(),
                    Footprint = p.CachedFootprint?.Name,
                    CenterX = GetAssemblyX(p.X, boardOriginX, useImperial),
                    CenterY = GetAssemblyY(p.Y, boardOriginY, useImperial),
                    Rot = GetAssemblyRot(p.Rot)
                };

                items.Add(assemblyItem);
            }

            return items;
        }

        string GetAssemblyX(double x, double brdOriginX, bool useImperial)//x is in mm
        {
            x -= brdOriginX;

            if (useImperial)
                x /= 25.4;

            return x.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        string GetAssemblyY(double y, double brdOriginY, bool useImperial)//y is in mm
        {
            y = brdOriginY - y;

            if (useImperial)
                y /= 25.4;

            return y.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        string GetAssemblyRot(double rot)
        {
            if (rot != 0.00d)
            {
                rot = -Math.Round(rot, 2);
            }

            return rot.ToString("0", CultureInfo.InvariantCulture);
        }

        public Task<DynamicList> GetOutputData(IBoardDesigner board, IList<AssemblyOutputColumn> columns)
        {
            return Task.Run(() =>
            {
                var src = GetPickAndPlaceList(board);

                var propertyNames = columns.Where(c => c.Show)
                                           .Select(c => new PropertyNameDisplayMapping
                                           {
                                               PropertyName = c.ColumnName,
                                               DisplayName = c.Header
                                           }
                                                  )
                                           .ToList();

                var result = src.Select(b => new CustomType(propertyNames, b))
                                .ToList();

                return new DynamicList(propertyNames, result);
            });
        }


    }

    public class BoardAssemblyHelper
    {
        public IList<BoardGlobalLayerOutput> GetAssemblyLayers(IBoardDesigner board, IList<BoardGlobalLayerOutput> allBuildLayers)
        {
            var assemblyParts = GetBoardAssemblyParts(board);

            var validLayerTypes = new[] {
                                         LayerType.Signal,
                                         LayerType.SilkScreen,
                                         LayerType.Mechanical,
                                         LayerType.Generic
                                        };


            var layers = allBuildLayers.Where(l => validLayerTypes.Contains(l.Layer.LayerType)).ToList();

            var topLayers = layers.Where(l => l.Layer.IsTopLayer).ToList();
            var bottomLayers = layers.Where(l => l.Layer.IsBottomLayer).ToList();

            //todo: need to add abstraction of BoardProperties and BuildOptions
            foreach (var layerPair in ( (BoardDesignerFileViewModel)board ).BoardProperties.LayerPairs)
            {
                var layerTop = layers.FirstOrDefault(l => l.Layer.LayerId == layerPair.LayerStart.LayerId);
                var layerBottom = layers.FirstOrDefault(l => l.Layer.LayerId == layerPair.LayerEnd.LayerId);

                if (layerTop != null)
                {
                    topLayers.Add(layerTop);
                }

                if (layerBottom != null)
                {
                    bottomLayers.Add(layerBottom);
                }
            }

            var topParts = assemblyParts.Where(p => p.Placement == FootprintPlacement.Top).ToList();
            var bottomParts = assemblyParts.Where(p => p.Placement == FootprintPlacement.Bottom).ToList();

            var topLayer = BuildLayer(topLayers, topParts, true);
            var bottomLayer = BuildLayer(bottomLayers, bottomParts, false);

            var drawingsLayers = new List<BoardGlobalLayerOutput>();
            drawingsLayers.Add(topLayer);
            drawingsLayers.Add(bottomLayer);

            return drawingsLayers;

        }

        private BoardGlobalLayerOutput BuildLayer(IList<BoardGlobalLayerOutput> srcLayers, IList<GlobalPickAndPlacePrimitive> layerParts, bool isTop)
        {

            var addItems = new List<GlobalPrimitive>();
            foreach (var part in layerParts)
            {
                if (string.IsNullOrEmpty(part.PartName))
                    continue;

                foreach (var layer in srcLayers)
                {
                    //todo: review this: we add only pin1 pin from signal; from the other layers only the outline
                    foreach (var item in layer.AddItems)//.Where(p => p is GlobalArcPrimitive || p is GlobalLinePrimitive))
                    {
                        if (item.Tags.TryGetValue(nameof(GlobalStandardPrimitiveTag.PartName), out var partNameValue))
                        {
                            var itemPartName = partNameValue as string;
                            if (!string.IsNullOrEmpty(itemPartName) && part.PartName == itemPartName)
                            {
                                //pin 1
                                if (layer.Layer.LayerType == LayerType.Signal
                                    && item.Tags.TryGetValue(nameof(GlobalStandardPrimitiveTag.PinNumber), out var pinNumberValue))
                                {
                                    var pinNumber = pinNumberValue as string;
                                    if (pinNumber == "1" || pinNumber == "A1")//? what is pin 1 for bga?
                                    {
                                        if (item is GlobalRectanglePrimitive pad)
                                        {
                                            part.Pin1Pos = new XPoint(pad.X, pad.Y);
                                        }
                                    }

                                    continue;
                                }

                                var addItem = false;
                                switch (layer.Layer.LayerType)
                                {
                                    case LayerType.SilkScreen:
                                        if (item is GlobalArcPrimitive || item is GlobalLinePrimitive)
                                        {
                                            item.Tags[nameof(GlobalStandardPrimitiveTag.Role)] = GlobalStandardPrimitiveRole.ComponentOutlineFootprint;
                                            addItem = true;
                                        }
                                        break;

                                    case LayerType.Mechanical:
                                    case LayerType.Generic:
                                        if (item is GlobalArcPrimitive || item is GlobalLinePrimitive)
                                        {
                                            item.Tags[nameof(GlobalStandardPrimitiveTag.Role)] = GlobalStandardPrimitiveRole.ComponentOutlineCourtyard;
                                            addItem = true;
                                        }
                                        break;
                                }

                                if (addItem)
                                    part.Items.Add(item);
                            }
                        }
                    }
                }

                addItems.Add(part);
            }

            var srcLayer = srcLayers[0].Layer;
            var drawingLayer = new LayerDesignerItem(null)
            {
                LayerType = isTop ? LayerType.ComponentTop : LayerType.ComponentBottom,
                GerberExtension = srcLayer.GerberExtension,
                LayerName = $"pick-and-place-{( isTop ? "Top" : "Bottom" )}",
            };

            return new BoardGlobalLayerOutput
            {
                Layer = drawingLayer,
                BoardOutline = srcLayers[0].BoardOutline,
                AddItems = addItems
            };
        }

        private IList<GlobalPickAndPlacePrimitive> GetBoardAssemblyParts(IBoardDesigner board)
        {
            var items = new List<GlobalPickAndPlacePrimitive>();

            var parts = board.GetFootprints()
                                    .OrderBy(f => f.PartName, new IndexedNameComparer())
                                    .ToList();

            var project = board.GetCurrentProjectInfo();

            foreach (var p in parts)
            {
                var assemblyItem = new GlobalPickAndPlacePrimitive
                {
                    PartName = p.PartName,
                    Placement = p.Placement,
                    FootprintName = p.CachedFootprint?.Name,
                    Center = new XPoint(p.X, p.Y),
                    Rot = p.Rot
                };

                var bomItem = p.FootprintPrimitive.GetBomItem(project);

                if (bomItem != null)
                {
                    assemblyItem.Manufacturer = bomItem.Manufacturer;
                    assemblyItem.Mpn = bomItem.MPN;
                    assemblyItem.SupplierName = bomItem.Supplier;
                    assemblyItem.SupplierPartNumber = bomItem.Sku;
                }

                items.Add(assemblyItem);
            }

            return items;
        }
    }

}
