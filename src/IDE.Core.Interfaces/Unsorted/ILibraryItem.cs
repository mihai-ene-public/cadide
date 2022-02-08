using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ILibraryItem
    {
        
        
        long Id { get; set; }

        string Name { get; set; }

        string Namespace { get; set; }

        string Library { get; set; }

        bool IsLocal { get; }
    }
}
