using IDE.Core.BOM;
using IDE.Core.Build;
using IDE.Core.Common.Variables;
using IDE.Core.Excelon;
using IDE.Core.Gerber;
using IDE.Core.Interfaces;
using IDE.Core.PDF;
using IDE.Core.Presentation.Builders;
using IDE.Documents.Views;
using System.IO;
using System.IO.Compression;

namespace IDE.Core.Build
{
    public class BoardGlobalOutputBuilder
    {
        public async Task<BuildResult> Build(IBoardDesigner board)
        {
            var project = board.ProjectNode;
            var folderOutput = Path.Combine(project.GetItemFolderFullPath(), "!Output");
            var boardName = Path.GetFileNameWithoutExtension(board.FilePath);

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

            var outputFiles = new List<string>();
            //var gerberLayers = new List<GerberLayer>();
            //var excelonFiles = new List<ExcelonLayer>();

            var outputHelper = new BoardGlobalOutputHelper();
            var buildResult = outputHelper.Build(board, validLayerTypes);

            CreateOutputFolder(folderOutput);
            ClearOutputFolder(folderOutput, boardName);

            //gerber files
            foreach (var layer in buildResult.Layers)
            {
                var gerberBuilder = new GerberLayerBuilder();
                var gerberPath = GetGerberFilePath(folderOutput, boardName, layer);
                var gerberBuildResult = await gerberBuilder.Build(board, layer, gerberPath);
                if (gerberBuildResult.Success)
                {
                    outputFiles.AddRange(gerberBuildResult.OutputFiles);
                }
            }

            //excelon
            foreach (var drillPair in buildResult.DrillLayers)
            {
                var ncLayer = new ExcelonLayerBuilder();
                var excelonFilePath = GetExcelonFilePath(folderOutput, boardName, drillPair);
                var ncResult = await ncLayer.Build(board, drillPair, excelonFilePath);
                if (ncResult.Success)
                {
                    outputFiles.AddRange(ncResult.OutputFiles);
                }
            }

            //create zip
            if (board.BuildOptions.GerberCreateZipFile)
            {
                var zipPath = Path.Combine(folderOutput, $"{boardName}.zip");
                using (var fs = new FileStream(zipPath, FileMode.Create))
                {
                    using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Create, false))
                    {
                        foreach (var file in outputFiles)
                        {
                            var entry = zipFile.CreateEntryFromFile(file, Path.GetFileName(file));
                        }
                    }
                }

                outputFiles.Add(zipPath);
            }

            //pdf
            var pdfOutput = new PdfBoardGlobalOutput();
            var pdfLayers = buildResult.Layers.Where(l => l.Layer.LayerType == LayerType.Signal);
            var pdfSavePath = Path.Combine(folderOutput, $"{boardName}.pdf");
            var pdfResult = await pdfOutput.Build(board, pdfLayers, pdfSavePath);

            if (pdfResult.Success)
            {
                outputFiles.AddRange(pdfResult.OutputFiles);
            }

            //bom
            var csvPath = GetCsvFilePath(folderOutput, boardName);
            var bomWriter = new BomOutputWriter();
            await bomWriter.Build(board, csvPath);

            outputFiles.Add(csvPath);

            //assembly pick and place
            var pickAndPlacePath = Path.Combine(folderOutput, $"{boardName}-PickAndPlace.csv");
            var pickAndPlaceSvc = new BoardAssemblyGlobalOutputPickAndPlaceService();
            await pickAndPlaceSvc.Build(board, pickAndPlacePath);

            outputFiles.Add(pickAndPlacePath);

            //assembly drawings
            var assemblyDrawingsPath = Path.Combine(folderOutput, $"{boardName} - Assembly Drawings.pdf");
            var assyDrawingsSvc = new BoardAssemblyDrawingsGlobalOutputService();
            var drawingsFile = await assyDrawingsSvc.Build(board, buildResult.Layers, assemblyDrawingsPath);
            outputFiles.Add(drawingsFile);

            return new BuildResult
            {
                Success = true,
                OutputFiles = outputFiles
            };
        }

        void CreateOutputFolder(string folderOutput)
        {
            Directory.CreateDirectory(folderOutput);
        }

        void ClearOutputFolder(string folderOutput, string brdName)
        {
            foreach (var file in Directory.GetFiles(folderOutput, $"{brdName}-*.*"))
            {
                //might be some in use...
                try { File.Delete(file); } catch { }
            }
        }

        private VariablesContext GetVariables(string boardName, BoardGlobalLayerOutput layer)
        {
            var variables = new VariablesContext();
            variables.Add(new Variable("boardName", boardName));
            variables.Add(new Variable("layerName", layer.Layer.LayerName));

            return variables;
        }

        private string GetGerberFilePath(string folderOutput, string boardName, BoardGlobalLayerOutput layer)
        {
            var layerItem = layer.Layer;
            var gerberFileName = $"{boardName}-{layerItem.LayerName}";
            if (!string.IsNullOrWhiteSpace(layerItem.GerberFileName))
            {
                var variables = GetVariables(boardName, layer);

                gerberFileName = variables.Replace(layerItem.GerberFileName);
            }

            var savePath = Path.Combine(folderOutput, $"{gerberFileName}{GetExtension(layerItem.GerberExtension)}");

            return savePath;
        }

        private string GetExcelonFilePath(string folderOutput, string boardName, BoardGlobalDrillPairOutput drillPair)
        {
            var savePath = Path.Combine(folderOutput, $"{boardName}-{drillPair.DrillPair}-DrillsAndSlots.txt");

            return savePath;
        }

        private string GetCsvFilePath(string folderOutput, string boardName)
        {
            return Path.Combine(folderOutput, $"{boardName}-BOM.csv");

        }

        private string GetExtension(string extension)
        {
            if (extension.StartsWith("."))
                return extension;
            return "." + extension.ToUpper();
        }
    }
}
