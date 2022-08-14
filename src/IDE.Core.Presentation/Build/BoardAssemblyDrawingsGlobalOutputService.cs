using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using IDE.Documents.Views;
using System.IO;

namespace IDE.Core.Build
{
    public class BoardAssemblyDrawingsGlobalOutputService
    {
        public async Task<string> Build(IBoardDesigner board, IList<BoardGlobalLayerOutput> allBuildLayers, string savePath)
        {
            var pdfLayers = GetAssemblyDrawingsLayers(board, allBuildLayers);

            var pdfOutput = new PdfBoardGlobalOutput();
            var pdfPath = await pdfOutput.Build(board, pdfLayers, savePath);

            return savePath;
        }

        public IList<BoardGlobalLayerOutput> GetAssemblyDrawingsLayers(IBoardDesigner board, IList<BoardGlobalLayerOutput> allBuildLayers)
        {
            var validLayerTypes = new[] {
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

            var topLayer = BuildLayer(topLayers, true);
            var bottomLayer = BuildLayer(bottomLayers, false);

            var drawingsLayers = new List<BoardGlobalLayerOutput>();
            drawingsLayers.Add(topLayer);
            drawingsLayers.Add(bottomLayer);

            return drawingsLayers;
        }

        private BoardGlobalLayerOutput BuildLayer(IList<BoardGlobalLayerOutput> srcLayers, bool isTop)
        {
            //creates a merge of all items from all source Layers

            var items = from layer in srcLayers
                        from item in layer.AddItems
                        select item;

            var srcLayer = srcLayers[0].Layer;
            var drawingLayer = new LayerDesignerItem(null)
            {
                LayerType = isTop ? LayerType.AssemblyDrawingTop : LayerType.AssemblyDrawingBottom,
                GerberExtension = srcLayer.GerberExtension,
                LayerName = $"assembly-drawings-{( isTop ? "Top" : "Bottom" )}",
            };

            return new BoardGlobalLayerOutput
            {
                Layer = drawingLayer,
                BoardOutline = srcLayers[0].BoardOutline,
                AddItems = items.ToList()
            };
        }
    }

}
