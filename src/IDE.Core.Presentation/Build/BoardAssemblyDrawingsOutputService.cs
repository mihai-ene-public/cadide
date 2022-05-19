using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using IDE.Documents.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
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
