using IDE.Core.Errors;

namespace IDE.Core.Interfaces
{
    public class RuleCheckResult
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public CanvasLocation Location { get; set; }
    }
}
