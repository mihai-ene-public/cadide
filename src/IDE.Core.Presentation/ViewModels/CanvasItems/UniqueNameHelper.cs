using System.Collections.Generic;
using System.Linq;
//using IDE.Documents.DiagramDesigner.Messenger;

namespace IDE.Core.Designers
{
    public class UniqueNameHelper
    {
        public static string GetNextUniqueName(IList<string> existingNames, string prefix = "P")
        {
            var currentPartName = prefix + "1";
            var currentIndex = 1;
            while (existingNames.Any(p => p == currentPartName))
            {
                currentIndex++;
                currentPartName = prefix + currentIndex;
            }

            return currentPartName;
        }
    }
}
