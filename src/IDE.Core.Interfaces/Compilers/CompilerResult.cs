using System.Collections.Generic;
using IDE.Core.Interfaces;

namespace IDE.Core.Interfaces.Compilers;

public class CompilerResult
{
    public bool Success { get; set; }
    public IList<IErrorMessage> Errors { get; set; } = new List<IErrorMessage>();
}
