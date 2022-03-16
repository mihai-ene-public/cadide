using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation.Solution
{
    public interface ISolutionRepository
    {
        IList<IProjectDocument> GetSolutionProjects(string solutionFilePath);
    }
}
