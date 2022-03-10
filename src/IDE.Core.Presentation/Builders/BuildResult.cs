using System.Collections.Generic;

namespace IDE.Core.Presentation.Builders
{
    public class BuildResult
    {
        public bool Success { get; set; }
        public IList<string> OutputFiles { get; set; } = new List<string>();
    }
}
