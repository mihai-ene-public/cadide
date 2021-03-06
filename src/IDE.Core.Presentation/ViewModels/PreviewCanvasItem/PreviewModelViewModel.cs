using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class PreviewModelViewModel : PreviewLibraryItemViewModel
    {
        public override void PreviewDocument(LibraryItem libraryItem, ISolutionProjectNodeModel projectModel)
        {
           if(libraryItem is ModelDocument model)
                    PreviewModelDocument(model);
        }

        //public DesignerViewMode DesignerViewMode => DesignerViewMode.ViewMode3D;

        private void PreviewModelDocument(ModelDocument modelDocument)
        {
            //switch to 3D
            //throw new NotImplementedException();
            var primitives = new List<ISelectableItem>();
            if (modelDocument.Items != null)
            {
                foreach (var primitive in modelDocument.Items)
                {
                    var canvasItem = primitive.CreateDesignerItem();
                    primitives.Add(canvasItem);
                }
            }

            LoadPrimitives(primitives);
            canvasModel.OnPropertyChanged(nameof(canvasModel.Items));
        }
    }
}
