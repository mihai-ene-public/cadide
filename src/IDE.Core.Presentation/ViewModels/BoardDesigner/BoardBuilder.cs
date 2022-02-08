using IDE.Core;
using IDE.Core.BOM;
using IDE.Core.Build;
using IDE.Core.Excelon;
using IDE.Core.Gerber;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class BoardBuilder
    {
        public async Task Build(IBoardDesigner board, string folderOutput, string brdName)
        {
            //layer types: stackup: signal, plane;
            //             milling, silkscreen, soldermask, pasteMask 
            var validLayerTypes = new[] {  LayerType.Signal,
                                         LayerType.Plane,
                                         LayerType.SolderMask,
                                         LayerType.PasteMask,
                                         LayerType.SilkScreen,
                                         LayerType.Mechanical,
                                         LayerType.Generic,
                                         LayerType.BoardOutline
                                            };


            var gerberLayers = new List<GerberLayer>();
            var excelonFiles = new List<ExcelonLayer>();


            var outputHelper = new BoardOutputHelper(validLayerTypes, board);
            var layers = outputHelper.BuildOutputLayers();

            foreach (var layer in layers)
            {
                var gl = new GerberLayer(board, layer.AddItems.ToList(), layer.ExtractItems?.ToList());
                gl.PrepareLayer(layer.Layer);
                gerberLayers.Add(gl);
            }


            board.OutputFiles.Clear();
            //clean output folder
            
            if (Directory.Exists(folderOutput))
            {
                foreach (var file in Directory.GetFiles(folderOutput, $"{brdName}-*.*"))
                {
                    //might be some in use...
                    try { File.Delete(file); } catch { }
                }
            }
            //we now create the output
            //based on options we could create a zip per board
            foreach (var gerberLayer in gerberLayers)
            {
                await gerberLayer.BuildLayer();
                board.OutputFiles.AddRange(gerberLayer.OutputFiles);
            }

            //excelon
            foreach (var drillPair in outputHelper.DrillLayers)
            {
                excelonFiles.Add(new ExcelonLayer(board, drillPair));
            }
            foreach (var excelonFile in excelonFiles)
            {
                await excelonFile.Build();
                board.OutputFiles.AddRange(excelonFile.OutputFiles);
            }

            //create zip
            if (board.BuildOptions.GerberCreateZipFile)
            {
                var zipPath = Path.Combine(folderOutput, $"{brdName}.zip");
                using (var fs = new FileStream(zipPath, FileMode.Create))
                {
                    using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Create, false))
                    {
                        foreach (var file in board.OutputFiles)
                        {
                            var entry = zipFile.CreateEntryFromFile(file, Path.GetFileName(file));
                        }
                    }
                }

                board.OutputFiles.Add(zipPath);
            }

            var pdfOutput = new PdfBoardOutput();
            var pdfLayers = layers.Where(l => l.Layer.LayerType == LayerType.Signal);
            var pdfSavePath = Path.Combine(folderOutput, $"{brdName}.pdf");
            var pdfPath = await pdfOutput.Build(board, pdfLayers, pdfSavePath);

            board.OutputFiles.Add(pdfPath);


            //bom
            var bomWriter = new BomOutputWriter((BoardDesignerFileViewModel)board);
            await bomWriter.Build();

            board.OutputFiles.AddRange(bomWriter.OutputFiles);

            //assembly
            var assyWriter = new BoardAssemblyOutputService();
            await assyWriter.Build((BoardDesignerFileViewModel)board);

            board.OutputFiles.AddRange(assyWriter.OutputFiles);

        }

    }
}
