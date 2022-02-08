using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Text;


namespace IDE.Core.Presentation.Repository
{
    public class LibraryItemFinder : ILibraryItemFinder
    {
        public ComponentDocument FindComponent(string projectFilePath, string libraryName, long id, DateTime? lastModified = null)
        {
            throw new NotImplementedException();
        }

        public Footprint FindFootprint(string projectFilePath, long id)
        {
            throw new NotImplementedException();
        }

        public Footprint FindFootprint(string projectFilePath, string libraryName, long id, DateTime? lastModified = null)
        {
            throw new NotImplementedException();
        }

        public ModelDocument FindModel(string projectFilePath, string libraryName, long id, DateTime? lastModified = null)
        {
            throw new NotImplementedException();
        }

        public SchematicDocument FindSchematic(string projectFilePath, string libraryName, long id, DateTime? lastModified = null)
        {
            throw new NotImplementedException();
        }

        public Symbol FindSymbol(string projectFilePath, long id)
        {
            throw new NotImplementedException();
        }

        public Symbol FindSymbol(string projectFilePath, string libraryName, long id, DateTime? lastModified = null)
        {
            throw new NotImplementedException();
        }
    }
}
