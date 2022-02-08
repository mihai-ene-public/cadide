using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IErrorMessage
    {
        MessageSeverity Severity { get; set; }

        string Description { get; set; }

        string Project { get; set; }

        //filename the error occurs
        string File { get; set; }

        IErrorLocation Location { get; set; }
    }
}
