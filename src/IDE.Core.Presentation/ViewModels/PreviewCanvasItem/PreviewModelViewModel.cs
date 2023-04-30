using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Common;
using System.Collections.Generic;

namespace IDE.Documents.Views;

public class PreviewModelViewModel : PreviewLibraryItemViewModel
{
    public override void PreviewDocument(LibraryItem libraryItem)
    {
       if(libraryItem is ModelDocument model)
                PreviewModelDocument(model);
    }

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
    }
}
