using IDE.Core.Storage;
using System.Collections.Generic;

namespace IDE.Core
{
    public class SolutionManager
    {
        static SolutionManager instance;
        public static SolutionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SolutionManager();

                return instance;
            }
        }

        public static SolutionDocument Solution { get; private set; }

        public static void CreateNewSolution(string firstProjectName)
        {
            var solution = new SolutionDocument();
            solution.Children = new List<ProjectBaseFileRef>
                    {
                         new SolutionProjectItem
                         {
                             RelativePath = $@"{firstProjectName}/{firstProjectName}{ProjectDocument.ProjectExtension}"
                         },
                    };

            Solution = solution;
        }

        public static string SolutionFilePath { get; set; }

        public void SaveSolution(string slnFilePath)
        {
            XmlHelper.Save(Solution, slnFilePath);
        }

        public static void SaveSolution()
        {
           Instance.SaveSolution(SolutionFilePath);
        }

        public static SolutionDocument LoadSolution(string slnFilePath)
        {
            Solution = XmlHelper.Load<SolutionDocument>(slnFilePath);
            SolutionFilePath = slnFilePath;

            return Solution;
        }

        public static void CloseSolution()
        {
            Solution = null;
            SolutionFilePath = null;
        }
          
        public static bool IsSolutionOpen()
        {
            return Solution != null;
        }
    }
}
