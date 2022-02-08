using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Units;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views
{
    public class PreviewSymbolViewModel : PreviewLibraryItemViewModel
    {
        public PreviewSymbolViewModel()
        {
            var docSize = 25.4 * 10;
            var halfSize = docSize * 0.5;
            canvasModel.DocumentWidth = docSize;
            canvasModel.DocumentHeight = docSize;
            canvasModel.Origin = new XPoint(halfSize, halfSize);

            ((CanvasGrid)canvasModel.CanvasGrid).GridSizeModel.SelectedItem = new MilUnit(50);
        }

        public override void PreviewDocument(LibraryItem libraryItem, ISolutionProjectNodeModel projectModel)
        {
           if(libraryItem is Symbol symbol)
                PreviewSymbolDocument(symbol);
        }

        private void PreviewSymbolDocument(Symbol symbol)
        {
            var primitiveItems = symbol.GetDesignerPrimitiveItems();

            LoadPrimitives(primitiveItems);
        }
    }
}
