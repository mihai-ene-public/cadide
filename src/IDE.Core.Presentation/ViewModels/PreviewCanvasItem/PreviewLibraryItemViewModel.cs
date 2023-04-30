using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Linq;

namespace IDE.Documents.Views;

//the purpose of this class is to load a library item (symbol, footprint, etc) into canvasModel
public abstract class PreviewLibraryItemViewModel : PreviewCanvasViewModel
{
    public PreviewLibraryItemViewModel()
    {

    }

    protected ProjectInfo _project;

    protected void LoadPrimitives(IList<ISelectableItem> primitives)
    {
        Items = primitives;
    }
    internal void SetProject(ProjectInfo project)
    {
        _project = project;
    }
    public abstract void PreviewDocument(LibraryItem libraryItem);

    public static PreviewLibraryItemViewModel CreateFromDocument(LibraryItem libraryItem)
    {
        if (libraryItem == null)
            return null;

        var mapping = new Dictionary<Type, Type>
        {
            { typeof(Symbol), typeof(PreviewSymbolViewModel) },
            { typeof(Footprint), typeof(PreviewFootprintViewModel) },
            { typeof(ModelDocument), typeof(PreviewModelViewModel) },
            { typeof(SchematicDocument), typeof(PreviewSchematicViewModel) }
        };

        var libraryItemType = libraryItem.GetType();
        if (mapping.ContainsKey(libraryItemType))
        {
            var model = (PreviewLibraryItemViewModel)Activator.CreateInstance(mapping[libraryItemType]);

            return model;
        }

        throw new NotSupportedException();
    }
}
