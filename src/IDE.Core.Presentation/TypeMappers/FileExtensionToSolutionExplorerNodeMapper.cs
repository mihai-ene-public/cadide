using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace IDE.Core.ViewModels
{
    public class FileExtensionToSolutionExplorerNodeMapper :  IFileExtensionToSolutionExplorerNodeMapper
    {
        public FileExtensionToSolutionExplorerNodeMapper()
        {
            CreateMappings();
        }

        protected Dictionary<string, Type> typeMappings = new Dictionary<string, Type>();

       

        public void AddMapping(string firstType, Type secondType)
        {
            if (typeMappings.ContainsKey(firstType))
                return;

            typeMappings.Add(firstType, secondType);
        }

        public Type GetMapping(string extension)
        {
            if (typeMappings.TryGetValue(extension, out Type mappedType))
                return mappedType;

            return typeof(ProjectGenericFileNodeModel);
        }

        protected void CreateMappings()
        {
            AddMapping("board", typeof(ProjectBoardNodeModel));
            AddMapping("model", typeof(ProjectModelNodeModel));

            AddMapping("component", typeof(ProjectComponentNodeModel));
            AddMapping("diagram", typeof(ProjectDiagramNodeModel));
           // AddMapping(typeof(ProjectFolderFile), typeof(ProjectFolderNodeModel));
            AddMapping("footprint", typeof(ProjectFootprintNodeModel));
            //AddMapping(typeof(ProjectGenericFile), typeof(ProjectGenericFileNodeModel));
            AddMapping("schematic", typeof(ProjectSchematicNodeModel));
            AddMapping("symbol", typeof(ProjectSymbolNodeModel));
            AddMapping("font", typeof(ProjectFontNodeModel));
        }

        public ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(string fileExtension)
        {
            fileExtension = fileExtension.Replace(".", "");
            var mappedType = GetMapping(fileExtension);


            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as ISolutionExplorerNodeModel;

                if (nodeModel != null)
                {
                    //   nodeModel.FileItem = fileItem;

                    //if (nodeModel is ISolutionProjectNodeModel)
                    //    LoadProjectNodeModel(nodeModel, fileItem.RelativePath);

                    return nodeModel;
                }
            }

            throw new NotSupportedException();

            //return null;
        }
    }
}
