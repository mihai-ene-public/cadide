using IDE.Core.Common.Utilities;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.IO;

namespace IDE.Core.ViewModels
{
    public class SolutionExplorerNodeMapper : GenericMapper, ISolutionExplorerNodeMapper
    {
        public SolutionExplorerNodeMapper() : base()
        {
        }

        protected override void CreateMappings()
        {
            //AddMapping(typeof(ProjectBoardFile), typeof(ProjectBoardNodeModel));
            //AddMapping(typeof(ProjectModelFile), typeof(ProjectModelNodeModel));

            //AddMapping(typeof(ProjectComponentFile), typeof(ProjectComponentNodeModel));
            //AddMapping(typeof(ProjectDiagramFile), typeof(ProjectDiagramNodeModel));
            //AddMapping(typeof(ProjectFolderFile), typeof(ProjectFolderNodeModel));
            //AddMapping(typeof(ProjectFootprintFile), typeof(ProjectFootprintNodeModel));
            //AddMapping(typeof(ProjectGenericFile), typeof(ProjectGenericFileNodeModel));
            //AddMapping(typeof(ProjectSchematicFile), typeof(ProjectSchematicNodeModel));
            //AddMapping(typeof(ProjectSymbolFile), typeof(ProjectSymbolNodeModel));
            //AddMapping(typeof(ProjectFontFile), typeof(ProjectFontNodeModel));
            AddMapping(typeof(GroupFolderItem), typeof(SolutionVirtualFolderNodeModel));
            AddMapping(typeof(SolutionProjectItem), typeof(SolutionProjectNodeModel));//todo: this loads in a different way!!!

        }

        public ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(IProjectFileRef fileItem)
        {
            var mappedType = GetMapping(fileItem.GetType());
            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as ISolutionExplorerNodeModel;

                if (nodeModel != null)
                {
                 //   nodeModel.FileItem = fileItem;

                    if (nodeModel is ISolutionProjectNodeModel projectModel)
                        LoadProjectNodeModel(projectModel, fileItem);

                    return nodeModel;
                }
            }

            throw new NotSupportedException();

           //return null;
        }

        void LoadProjectNodeModel(ISolutionProjectNodeModel projectModel, IProjectFileRef fileItem)
        {
            var relativePath = fileItem.RelativePath;
            var projectPath = Path.Combine(Path.GetDirectoryName(SolutionManager.SolutionFilePath), relativePath);
            var projDoc = ProjectDocument.Load(projectPath.Replace(@"/", @"\"));

            projectModel.Document = projDoc;
            projectModel.FileItem = fileItem;
            projectModel.Load(projectPath);
        }
    }
}
