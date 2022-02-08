using System.IO;
using System.Linq;
using IDE.Core.Interfaces;

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

            var refs = ProjectNode.Project.References.Select(r => new ProjectReferenceNodeModel { Document = r });
            AddChildren(refs);

            UpdateReferences();
        }

        void UpdateReferences()
        {
            //if "References" folder does not exist, we need to update refeernces because it was just loaded from a template
            var referencesFolder = Path.Combine(ProjectNode.GetItemFolderFullPath(), "References");
            if (Directory.Exists(referencesFolder))
                return;

            var h = new ProjectReferencesHelper();
            h.UpdateReferences(Children);
        }
    }
}
