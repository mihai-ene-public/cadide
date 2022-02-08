using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IProjectDocument
    {
        ProjectOutputType OutputType { get; set; }

        ProjectProperties Properties { get; set; }

        List<ProjectDocumentReference> References { get; set; }

        string FilePath { get; set; }



        void Save();
    }

  
    public interface IProjectDocumentReference
    {

    }
}
