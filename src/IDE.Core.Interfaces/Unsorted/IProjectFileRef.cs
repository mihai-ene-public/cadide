using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IProjectFileRef
    {
        string RelativePath { get; set; }

        ProjectBaseFileRef Clone();
    }
}
