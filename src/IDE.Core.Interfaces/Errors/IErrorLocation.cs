using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IDE.Core.Interfaces
{
    public interface IErrorLocation
    {
        IFileBaseViewModel File { get; set; }

       // Point? Location { get; set; }
    }
}
