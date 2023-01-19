using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IProjectProperties
    {
        string BuildOutputFileName { get; set; }
        string BuildOutputFolderPath { get; set; }
        string BuildOutputNamespace { get; set; }
        string Company { get; set; }
        string Description { get; set; }
        string Product { get; set; }


        string Title { get; set; }
        string Version { get; set; }
        List<Property> Properties { get; set; }
    }
}
