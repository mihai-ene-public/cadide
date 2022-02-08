using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Presentation.Repository
{
    public interface ILibraryItemFinder//ILibraryItemRepository
    {
        //ILibraryItem FindObject(TemplateType type, long id);
        Footprint FindFootprint(string projectFilePath, long id);

        Symbol FindSymbol(string projectFilePath, long id);

        //ILibraryItem FindObject(TemplateType type, string libraryName, long id, DateTime? lastModified = null)

        Symbol FindSymbol(string projectFilePath, string libraryName, long id, DateTime? lastModified = null);
        
        Footprint FindFootprint(string projectFilePath, string libraryName, long id, DateTime? lastModified = null);
                
        ModelDocument FindModel(string projectFilePath, string libraryName, long id, DateTime? lastModified = null);
        
        ComponentDocument FindComponent(string projectFilePath, string libraryName, long id, DateTime? lastModified = null);
        
        SchematicDocument FindSchematic(string projectFilePath, string libraryName, long id, DateTime? lastModified = null);
    }
}
