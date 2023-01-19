using IDE.Core.Common;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Builders;
using IDE.Core.Presentation.Compilers;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDE.Core.ViewModels
{
    public class SolutionProjectNodeModel : FilesContainerNodeModel, ISolutionProjectNodeModel
    {
        protected override string GetNameInternal()
        {
            return Path.GetFileNameWithoutExtension(fileFullPath);
        }

        public ProjectOutputType ProjectOutputType { get; private set; }

        public override void Load(string filePath)
        {
            //load references and files in this project
            base.Load(filePath);

            var project = XmlHelper.Load<ProjectDocument>(filePath);
            ProjectOutputType = project.OutputType;

            //references
            var referencesNode = new ProjectReferencesNodeModel();
            AddChild(referencesNode);
            referencesNode.Load(filePath);

            //folders
            var projectDirectoryPath = Path.GetDirectoryName(filePath);
            LoadFolder(projectDirectoryPath);

        }
    }
}
