using System.IO;
using IDE.Core.Storage;

namespace IDE.Core.ViewModels
{
    public class ProjectReferenceNodeModel : SolutionExplorerNodeModel
    {
        protected override string GetNameInternal()
        {
            if (Document != null)
                return Document.ToString();
            return "unknown";
        }

        protected override void DeleteItemInternal()
        {
            var projNode = ProjectNode;
            var proj = projNode.Project;
            proj.References.Remove((ProjectDocumentReference)Document);

            //remove .libref
            if (Document is LibraryProjectReference lib)
            {
                var libRefFile = Path.Combine(projNode.GetItemFolderFullPath(), "References", lib.LibraryName + ".libref");

                if (File.Exists(libRefFile))
                    File.Delete(libRefFile);
            }

            proj.Save();
        }

        public override void Load(string filePath)
        {
            base.Load(filePath);
        }
    }
}
