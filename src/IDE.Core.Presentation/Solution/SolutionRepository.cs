using System.Collections.Generic;
using System.IO;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Solution
{
    public class SolutionRepository : ISolutionRepository
    {
        public IList<IProjectDocument> GetSolutionProjects(string solutionFilePath)
        {
            var solution = XmlHelper.Load<SolutionDocument>(solutionFilePath);

            var projects = new List<IProjectDocument>();

            foreach (var child in solution.Children)
            {
                if (child is SolutionProjectItem projectItem)
                {
                    var projectPath = Path.Combine(Path.GetDirectoryName(SolutionManager.SolutionFilePath), projectItem.RelativePath);
                    var projDoc = ProjectDocument.Load(projectPath.Replace(@"/", @"\"));
                    projects.Add(projDoc);
                }
            }

            return projects;
        }
    }
}
