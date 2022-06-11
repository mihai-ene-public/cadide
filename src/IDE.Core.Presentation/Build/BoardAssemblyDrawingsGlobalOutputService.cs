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
            var validLayerTypes = new[] {
                                         LayerType.SilkScreen,
                                         LayerType.Mechanical,
                                         LayerType.Generic
                                        };


            var layers = allBuildLayers.Where(l => validLayerTypes.Contains(l.Layer.LayerType)).ToList();

            var topLayers = layers.Where(l => l.Layer.IsTopLayer).ToList();
            var bottomLayers = layers.Where(l => l.Layer.IsBottomLayer).ToList();

            //todo: need to add abstraction of BoardProperties and BuildOptions
            foreach (var layerPair in ((BoardDesignerFileViewModel)board).BoardProperties.LayerPairs)
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

            var pdfLayers = new List<BoardGlobalLayerOutput>();
            pdfLayers.Add(topLayer);
            pdfLayers.Add(bottomLayer);

            var pdfOutput = new PdfBoardGlobalOutput();
            var pdfPath = await pdfOutput.Build(board, pdfLayers, savePath);

            return savePath;
        }

        private BoardGlobalLayerOutput BuildLayer(IList<BoardGlobalLayerOutput> srcLayers)
        {
            //creates a merge of all items from all source Layers

            var items = from layer in srcLayers
                        from item in layer.AddItems
                        select item;

            return new BoardGlobalLayerOutput
            {
                Layer = srcLayers[0].Layer,//?
                AddItems = items.ToList()
            };
        }
    }

}
