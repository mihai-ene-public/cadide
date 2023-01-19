using IDE.Core.Common.Utilities;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using System;
using System.IO;

namespace IDE.Core.ViewModels
{
    public class SolutionExplorerNodeMapper : GenericMapper, ISolutionExplorerNodeMapper
    {
        public SolutionExplorerNodeMapper() 
            : base()
        {
        }

        protected override void CreateMappings()
        {
            AddMapping(typeof(GroupFolderItem), typeof(SolutionVirtualFolderNodeModel));
            AddMapping(typeof(SolutionProjectItem), typeof(SolutionProjectNodeModel));//todo: this loads in a different way!!!
        }

        public ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(IProjectFileRef fileItem, string solutionFolder)
        {
            var mappedType = GetMapping(fileItem.GetType());
            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as ISolutionExplorerNodeModel;

                if (nodeModel != null)
                {
                    if (nodeModel is ISolutionProjectNodeModel projectModel)
                    {
                        var relativePath = fileItem.RelativePath;
                        var projectPath = Path.Combine(solutionFolder, relativePath);

                        projectModel.Load(projectPath);
                    }

                    return nodeModel;
                }
            }

            throw new NotSupportedException();
        }

    }
}
