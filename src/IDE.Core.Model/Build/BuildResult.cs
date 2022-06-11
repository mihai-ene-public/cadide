using System.Collections.Generic;

namespace IDE.Core.Build;

public class BuildResult
{
    public bool Success { get; set; }
    public IList<string> OutputFiles { get; set; } = new List<string>();
}
