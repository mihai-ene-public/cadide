using System.IO;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.ViewModels
{
    public class ProjectReferencesNodeModel : SolutionExplorerNodeModel, IProjectReferencesNodeModel
    {
        public ProjectReferencesNodeModel()
        {
            IsExpanded = false;
        }

        protected override string GetNameInternal()
        {
            return "References";
        }

        public override void Load(string filePath)
        {
            Children.Clear();

            base.Load(filePath);

            var projectDoc = XmlHelper.Load<ProjectDocument>(filePath);
            var refs = projectDoc.References.Select(r => new ProjectReferenceNodeModel { FileName = r.ToString() });
            AddChildren(refs);

        }

    }
}
