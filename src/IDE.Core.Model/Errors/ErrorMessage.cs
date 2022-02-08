using IDE.Core.Documents;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Errors
{

    public class ErrorMessage : IErrorMessage
    {
        public MessageSeverity Severity { get; set; }

        public string Description { get; set; }

        public string Project { get; set; }

        public string File { get; set; }

        public IErrorLocation Location { get; set; }
    }


}
