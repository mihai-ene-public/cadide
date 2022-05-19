using IDE.Core.Interfaces;
using IDE.Documents.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Core.Build
{
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
}
