using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using IDE.Core.Storage;
using IDE.Documents.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
    public class BoardOutputHelper
    {
        public BoardOutputHelper(LayerType[] _validLayerTypes, IBoardDesigner boardModel)
        {
            validLayerTypes = _validLayerTypes;
            board = boardModel;
        }

        LayerType[] validLayerTypes;
        IBoardDesigner board;

        IEnumerable<ISelectableItem> canvasItems;

        List<ISelectableItem> footprintItems;
        List<ICanvasItem> padItems;
        IEnumerable<IViaCanvasItem> viaItems;
        IEnumerable<ICanvasItem> viaPads;

        IEnumerable<ICanvasItem> millingItems;
        List<IHoleCanvasItem> drillItems;

        IList<BoardDrillPairOutput> drillLayers;
        public IEnumerable<BoardDrillPairOutput> DrillLayers => drillLayers;


        public List<BoardLayerOutput> BuildOutputLayers()
        {
            EnumerateItems();

            var layers = BuildLayers();

            BuildDrillPairs();

            return layers;
        }

        void EnumerateItems()
        {
            var canvasModel = board.CanvasModel;

            canvasItems = (List<ISelectableItem>)canvasModel.GetItems();

            //smd and pad
            var footprints = canvasItems.OfType<FootprintBoardCanvasItem>().ToList();
            footprintItems = (from fp in footprints
                              from p in fp.Items
                              select (ISelectableItem)((p as BaseCanvasItem).Clone())).ToList();
            //we add designators as text items
            var designators = (from fp in footprints
                               where fp.ShowName
                               select (BaseCanvasItem)fp.Designator.Clone())
                              .ToList();

            //designators.ForEach(d =>
            //{
            //    var fp = d.ParentObject as FootprintBoardCanvasItem;
            //    d.Translate(fp.PartNamePosition.X, fp.PartNamePosition.Y);
            //});
            footprintItems.AddRange(designators);

            padItems = footprintItems.OfType<IPadCanvasItem>().Cast<ICanvasItem>().ToList();
            padItems.AddRange(canvasItems.OfType<IPadCanvasItem>().Cast<ICanvasItem>());
            viaItems = canvasItems.OfType<IViaCanvasItem>().ToList();

            viaPads = viaItems.Select(v => new CircleBoardCanvasItem
            {
                BorderWidth = 0,
                IsFilled = true,
                Diameter = v.Diameter,
                X = v.X,
                Y = v.Y,
                ParentObject = v
            }).Cast<ICanvasItem>().ToList();
            //todo: we must assign a layer for the drill pair; for now, top and bottom


            //drills from pads and holes
            drillItems = padItems.OfType<PadThtCanvasItem>().Select(p =>
            new HoleCanvasItem
            {
                Drill = p.Drill,
                X = p.X + p.Hole.X,
                Y = p.Y + p.Hole.Y,
                DrillType = p.Hole.DrillType,
                Rot = p.Hole.Rot,
                Height = p.Hole.Height,
                ParentObject = p.ParentObject
            }
            ).Cast<IHoleCanvasItem>()
            .Union(footprintItems.OfType<HoleCanvasItem>()) //holes from footprints
            .Union(canvasItems.OfType<HoleCanvasItem>())
            //vias from top-bottom
            .Union(viaItems.Where(v => v.DrillPair?.LayerStart?.LayerId == LayerConstants.SignalTopLayerId
                                    && v.DrillPair.LayerEnd?.LayerId == LayerConstants.SignalBottomLayerId)
                           .Select(v =>
              new HoleCanvasItem
              {
                  Drill = v.Drill,
                  X = v.X,
                  Y = v.Y,
                  ParentObject = v
              }))
           .ToList();

            millingItems = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                                .Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId)
                                                .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>().Where(c => c.Layer != null && c.Layer.LayerId == LayerConstants.MultiLayerMillingId))
                                                .OfType<ICanvasItem>().ToList();
        }

        List<BoardLayerOutput> BuildLayers()
        {
            var outputLayers = new List<BoardLayerOutput>();

            var itemsGroupedByLayer = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                              .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>())
                                              .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                              .GroupBy(p => p.LayerId);

            var layers = board.LayerItems.Where(l => l.Plot && validLayerTypes.Contains(l.LayerType)).ToList();

            foreach (var layer in layers)
            {
                var layerId = layer.LayerId;
                var layerGroup = itemsGroupedByLayer.FirstOrDefault(g => g.Key == layerId);//.Cast<ICanvasItem>();

                IEnumerable<ICanvasItem> layerItems = layerGroup?.Cast<ICanvasItem>();

                switch (layer.LayerType)
                {
                    case LayerType.Signal:
                        outputLayers.Add(BuildSignalLayer(layer, layerItems));
                        break;

                    case LayerType.SilkScreen:
                        outputLayers.Add(BuildSilkScreenLayer(layer, layerItems));
                        break;

                    case LayerType.SolderMask:
                        outputLayers.Add(BuildSolderMaskLayer(layer, layerItems));
                        break;

                    case LayerType.PasteMask:
                        outputLayers.Add(BuildPasteMaskLayer(layer, layerItems));
                        break;

                    case LayerType.Plane:
                        outputLayers.Add(BuildPlaneLayer(layer, layerItems));
                        break;

                    case LayerType.Mechanical:
                        outputLayers.Add(BuildMechanicalLayer(layer, layerItems));
                        break;

                    case LayerType.Generic:
                        outputLayers.Add(BuildGenericLayer(layer, layerItems));
                        break;

                    default:
                        outputLayers.Add(BuildGenericLayer(layer, layerItems));
                        break;
                }
            }

            return outputLayers;
        }

        void BuildDrillPairs()
        {
            drillLayers = new List<BoardDrillPairOutput>();

            var topBottomPair = board.DrillPairs.FirstOrDefault(d => d.LayerStart.LayerId == LayerConstants.SignalTopLayerId
                                                                  && d.LayerEnd.LayerId == LayerConstants.SignalBottomLayerId);

            //top-bottom
            drillLayers.Add(new BoardDrillPairOutput
            {
                DrillPair = topBottomPair,
                DrillItems = drillItems,
                MillingItems = millingItems
            });

            //the rest of pairs

            //vias grouped by pair but not top-bottom
            var viasGrouped = viaItems.Where(v => v.DrillPair != topBottomPair).GroupBy(v => v.DrillPair);

            foreach (var viaGroup in viasGrouped)
            {
                var viaHoles = viaGroup.Select(v => new HoleCanvasItem
                {
                    Drill = v.Drill,
                    X = v.X,
                    Y = v.Y
                } as IHoleCanvasItem)
                .ToList();

                drillLayers.Add(new BoardDrillPairOutput
                {
                    DrillPair = viaGroup.Key,
                    DrillItems = viaHoles,
                    // MillingItems = millingItems
                });
            }
        }

        BoardLayerOutput BuildPlaneLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            var excludeItems = new List<ICanvasItem>();

            if (layerItems != null)
                addItems.AddRange(layerItems.OfType<ICanvasItem>());

            //drills from pads
            excludeItems.AddRange(drillItems);
            excludeItems.AddRange(millingItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems,
                ExtractItems = excludeItems
            };
        }

        BoardLayerOutput BuildPasteMaskLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();

            if (layerItems != null)
                addItems.AddRange(layerItems.OfType<ICanvasItem>());
            //drills from pads
            if (layer.IsTopLayer)
            {
                addItems.AddRange(padItems.Where(p => ((IPadCanvasItem)p).AutoGeneratePasteMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId)));
            }
            if (layer.IsBottomLayer)
            {
                addItems.AddRange(padItems.Where(p => ((IPadCanvasItem)p).AutoGenerateSolderMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId)));
            }
            if (layer.IsTopLayer || layer.IsBottomLayer)
            {
                addItems.AddRange(padItems.OfType<PadThtCanvasItem>());
            }


            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems,
            };
        }

        BoardLayerOutput BuildSolderMaskLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();

            if (layerItems != null)
                addItems.AddRange(layerItems.OfType<ICanvasItem>());
            //drills from pads
            //addItems.AddRange(drillItems);
            addItems.AddRange(millingItems);
            if (layer.IsTopLayer)
            {
                addItems.AddRange(drillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || (d.ParentObject as ViaCanvasItem)?.TentViaOnTop == false));

                addItems.AddRange(padItems.Where(p => ((IPadCanvasItem)p).AutoGenerateSolderMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId)));

                //tht pads that are placed on bottom
                addItems.AddRange(padItems.OfType<PadThtCanvasItem>()
                                          .Where(p => ((IPadCanvasItem)p).AutoGenerateSolderMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId)));

                addItems.AddRange(viaPads.Cast<ISelectableItem>().Where(v => (v.ParentObject as ViaCanvasItem)?.TentViaOnTop == false));
            }
            if (layer.IsBottomLayer)
            {
                addItems.AddRange(drillItems.Cast<ISelectableItem>().Where(d => d.ParentObject == null || (d.ParentObject as ViaCanvasItem)?.TentViaOnBottom == false));

                addItems.AddRange(padItems.Where(p => ((IPadCanvasItem)p).AutoGenerateSolderMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId)));

                //tht pads
                addItems.AddRange(padItems.OfType<PadThtCanvasItem>()
                                          .Where(p => ((IPadCanvasItem)p).AutoGenerateSolderMask && ((((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId)));

                addItems.AddRange(viaPads.Cast<ISelectableItem>().Where(v => (v.ParentObject as ViaCanvasItem)?.TentViaOnBottom == false));
            }
            //addItems.AddRange(viaPads);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems,
            };
        }

        BoardLayerOutput BuildSignalLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var copperItems = new List<ICanvasItem>();
            var copperExclude = new List<ICanvasItem>();
            if (layerItems != null)
            {
                var polygons = layerItems.OfType<IPolygonCanvasItem>();

                var fillPolygons = polygons.Where(p => p.PolygonType == PolygonType.Fill);

                //polygons that are not keepout
                copperItems.AddRange(fillPolygons);

                copperItems.AddRange(layerItems.Cast<ICanvasItem>().Except(polygons));
            }

            //smd pads on top and bottom
            if (layer.IsTopLayer)
            {
                copperItems.AddRange(padItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                                                      || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId));
            }
            if (layer.IsBottomLayer)
            {
                copperItems.AddRange(padItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                                                        || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId));
            }
            //tht pads
            if (layer.IsTopLayer || layer.IsBottomLayer)
            {
                copperItems.AddRange(padItems.OfType<PadThtCanvasItem>());
            }

            //via pads on bottom
            copperItems.AddRange(viaPads);

            copperExclude.AddRange(drillItems);
            copperExclude.AddRange(millingItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = copperItems,
                ExtractItems = copperExclude
            };
        }
        BoardLayerOutput BuildSilkScreenLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var silkItems = new List<ICanvasItem>();
            if (layerItems != null)
                silkItems.AddRange(layerItems);
            IEnumerable<ICanvasItem> pads = null;
            IEnumerable<ICanvasItem> excludeSilkItems = null;
            if (layer.IsTopLayer)
            {
                pads = padItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Top
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalTopLayerId);
            }
            if (layer.IsBottomLayer)
            {
                pads = padItems.Where(p => (((BaseCanvasItem)p).ParentObject as FootprintBoardCanvasItem)?.Placement == FootprintPlacement.Bottom
                                                    || ((SingleLayerBoardCanvasItem)p).LayerId == LayerConstants.SignalBottomLayerId);
            }
            if (pads != null)
                excludeSilkItems = pads.Union(drillItems).Union(millingItems);
            else
                excludeSilkItems = drillItems.Union(millingItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = silkItems,
                ExtractItems = excludeSilkItems
            };
        }

        BoardLayerOutput BuildMechanicalLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            if (layerItems != null)
                addItems.AddRange(layerItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems,
            };
        }

        BoardLayerOutput BuildGenericLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            if (layerItems != null)
                addItems.AddRange(layerItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems,
            };
        }
    }

    public class BoardAssemblyOutputService
    {
        public BoardAssemblyOutputService()
        {
        }


        public List<string> OutputFiles { get; private set; } = new List<string>();

        public async Task Build(BoardDesignerFileViewModel board)
        {
            //generate pick and place
            var pickAndPlaceSvc = new BoardAssemblyOutputPickAndPlaceService(board);
            var csvFilePath = await pickAndPlaceSvc.Build();

            OutputFiles.Add(csvFilePath);

            //generate assembly drawings
            var validLayerTypes = new[] {
                                         LayerType.SilkScreen,
                                         LayerType.Mechanical,
                                         LayerType.Generic
                                        };
            var assyDrawingsSvc = new BoardAssemblyDrawingsOutputService(board, validLayerTypes);
            var drawingsFile = await assyDrawingsSvc.Build();
            //todo: pdf name from outside
            OutputFiles.Add(drawingsFile);
        }
    }

    public class BoardAssemblyOutputPickAndPlaceService
    {
        public BoardAssemblyOutputPickAndPlaceService(BoardDesignerFileViewModel boardModel)
        {
            board = boardModel;
        }

        BoardDesignerFileViewModel board;

        public async Task<string> Build()
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;
            var columns = buildOptions.Assembly.PickAndPlaceColumns;

            var pickAndPlaceHelper = new AssemblyPickAndPlaceHelper();
            var list = await pickAndPlaceHelper.GetOutputData(board, columns);

            var csvPath = GetCsvFilePath();
            var csvWriter = new CsvWriter(buildOptions.Assembly.FieldSeparator);
            await csvWriter.WriteCsv(list, csvPath);

            return csvPath;
        }

        private string GetCsvFilePath()
        {
            var project = board.ProjectNode;
            if (project == null)
                return null;

            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var brdName = Path.GetFileNameWithoutExtension(board.FilePath);
            savePath = Path.Combine(savePath, $"{brdName}-PickAndPlace.csv");

            return savePath;
        }
    }

    public class AssemblyPickAndPlaceHelper
    {
        public List<AssemblyPickAndPlaceItemDisplay> GetPickAndPlaceList(BoardDesignerFileViewModel board)
        {
            var buildOptions = ((BoardBuildOptionsViewModel)board.BuildOptions).Assembly;

            var items = new List<AssemblyPickAndPlaceItemDisplay>();

            var boardRectangle = board.GetBoardRectangle();
            var boardOriginX = boardRectangle.BottomLeft.X;
            var boardOriginY = boardRectangle.BottomLeft.Y;
            var useImperial = buildOptions.PositionUnits == OutputUnits.inch;

            var parts = board.GetBoardFootprints();

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
            rot = -rot;
            return rot.ToString("0", CultureInfo.InvariantCulture);
        }

        public Task<DynamicList> GetOutputData(BoardDesignerFileViewModel board, IList<AssemblyOutputColumn> columns)
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

    public class CsvWriter
    {

        public CsvWriter(string separator = ",")
        {
            _separator = separator;
        }

        private string _separator;

        public Task WriteCsv(DynamicList list, string savePath)
        {
            return Task.Run(() =>
            {
                var csv = new StringBuilder();

                if (string.IsNullOrEmpty(_separator))
                    _separator = ",";

                //write header
                var properties = list.GetItemProperties(null);
                var pNames = properties.Cast<PropertyDescriptor>()
                                       .Select(p => @$"""{ (string.IsNullOrEmpty(p.DisplayName) ? p.Name : p.DisplayName)}""")
                                       .ToArray();
                
                csv.AppendLine(string.Join(_separator, pNames));

                //write lines
                foreach (var bomItem in list)
                {
                    var values = new List<string>();
                    foreach (var bomProp in bomItem.GetProperties().Cast<PropertyDescriptor>())
                    {
                        var propValue = bomProp.GetValue(bomItem);
                        if (propValue == null)
                            propValue = string.Empty;
                        values.Add($@"""{propValue}""");
                    }

                    csv.AppendLine(string.Join(_separator, values.ToArray()));
                }

                File.WriteAllText(savePath, csv.ToString());

            });
        }
    }

    public class BoardAssemblyDrawingsOutputService
    {
        public BoardAssemblyDrawingsOutputService(BoardDesignerFileViewModel boardModel, LayerType[] _validLayerTypes)
        {
            board = boardModel;
            validLayerTypes = _validLayerTypes;
        }

        LayerType[] validLayerTypes;
        BoardDesignerFileViewModel board;

        IEnumerable<ISelectableItem> canvasItems;

        List<ISelectableItem> footprintItems;

        public async Task<string> Build()
        {
            var buildOptions = (BoardBuildOptionsViewModel)board.BuildOptions;

            var layers = BuildOutputLayers();

            var topLayers = layers.Where(l => l.Layer.IsTopLayer).ToList();
            var bottomLayers = layers.Where(l => l.Layer.IsBottomLayer).ToList();

            foreach (var layerPair in board.BoardProperties.LayerPairs)
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

            var topLayer = BuildLayer(topLayers);
            var bottomLayer = BuildLayer(bottomLayers);

            var pdfLayers = new List<BoardLayerOutput>();
            pdfLayers.Add(topLayer);
            pdfLayers.Add(bottomLayer);

            var pdfOutput = new PdfBoardOutput();
            var pdfSavePath = GetPdfFilePath();
            var pdfPath = await pdfOutput.Build(board, pdfLayers, pdfSavePath);

            return pdfPath;
        }

        string GetPdfFilePath()
        {
            var f = board as IFileBaseViewModel;
            if (f == null)
                return null;
            var project = f.ProjectNode;
            if (project == null)
                return null;
            var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
            Directory.CreateDirectory(savePath);
            var brdName = Path.GetFileNameWithoutExtension(f.FilePath);
            savePath = Path.Combine(savePath, $"{brdName} - Assembly Drawings.pdf");

            return savePath;
        }

        BoardLayerOutput BuildLayer(IList<BoardLayerOutput> srcLayers)
        {
            var items = from layer in srcLayers
                        from item in layer.AddItems
                        select item;

            return new BoardLayerOutput
            {
                Layer = srcLayers[0].Layer,
                AddItems = items.ToList()
            };
        }

        List<BoardLayerOutput> BuildOutputLayers()
        {
            EnumerateItems();

            var layers = BuildLayers();

            return layers;
        }

        void EnumerateItems()
        {
            var canvasModel = board.CanvasModel;

            canvasItems = (List<ISelectableItem>)canvasModel.GetItems();

            //smd and pad
            var footprints = canvasItems.OfType<FootprintBoardCanvasItem>().ToList();
            footprintItems = (from fp in footprints
                              from p in fp.Items
                              select (ISelectableItem)((p as BaseCanvasItem).Clone())).ToList();
            //we add designators as text items
            var designators = (from fp in footprints
                               where fp.ShowName
                               select (BaseCanvasItem)fp.Designator.Clone())
                              .ToList();

            designators.ForEach(d =>
            {
                var fp = d.ParentObject as FootprintBoardCanvasItem;
                d.Translate(fp.PartNamePosition.X, fp.PartNamePosition.Y);
            });
            footprintItems.AddRange(designators);
        }

        List<BoardLayerOutput> BuildLayers()
        {
            var outputLayers = new List<BoardLayerOutput>();

            var itemsGroupedByLayer = canvasItems.OfType<SingleLayerBoardCanvasItem>()
                                              .Union(footprintItems.OfType<SingleLayerBoardCanvasItem>())
                                              .Where(p => validLayerTypes.Contains(p.GetLayerType()))
                                              .GroupBy(p => p.LayerId);

            var layers = board.LayerItems.Where(l => validLayerTypes.Contains(l.LayerType)).ToList();

            foreach (var layer in layers)
            {
                var layerId = layer.LayerId;
                var layerGroup = itemsGroupedByLayer.FirstOrDefault(g => g.Key == layerId);

                IEnumerable<ICanvasItem> layerItems = layerGroup?.Cast<ICanvasItem>();

                switch (layer.LayerType)
                {
                    case LayerType.SilkScreen:
                        outputLayers.Add(BuildSilkScreenLayer(layer, layerItems));
                        break;

                    case LayerType.Mechanical:
                        outputLayers.Add(BuildMechanicalLayer(layer, layerItems));
                        break;

                    case LayerType.Generic:
                        outputLayers.Add(BuilGenericLayer(layer, layerItems));
                        break;
                }
            }

            return outputLayers;
        }

        private BoardLayerOutput BuildSilkScreenLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            if (layerItems != null)
                addItems.AddRange(layerItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems
            };
        }

        private BoardLayerOutput BuildMechanicalLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            if (layerItems != null)
                addItems.AddRange(layerItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems
            };
        }

        private BoardLayerOutput BuilGenericLayer(ILayerDesignerItem layer, IEnumerable<ICanvasItem> layerItems)
        {
            var addItems = new List<ICanvasItem>();
            if (layerItems != null)
                addItems.AddRange(layerItems);

            return new BoardLayerOutput
            {
                Layer = layer,
                AddItems = addItems
            };
        }
    }
}
