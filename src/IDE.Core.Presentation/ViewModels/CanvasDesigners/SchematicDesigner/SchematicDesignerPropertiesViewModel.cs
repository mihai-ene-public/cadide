using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class SchematicDesignerPropertiesViewModel : BaseViewModel
    {
       

        public SchematicDesignerPropertiesViewModel(SchematicDesignerViewModel schematicDesigner)
        {
            _schematicDesigner = schematicDesigner;
        }

        private readonly SchematicDesignerViewModel _schematicDesigner;

        public SchematicDesignerViewModel Schematic => _schematicDesigner;
        public NetClassesManagerViewModel NetClassesModel { get; set; } = new NetClassesManagerViewModel();
        public PartsBOMViewModel PartsBOMViewModel { get; set; } = new PartsBOMViewModel();

        public IList<ISchematicRuleModel> Rules { get; private set; }// = new List<ISchematicRuleModel>();

        public void LoadFromSchematic(SchematicDocument schematic)
        {
            NetClassesModel.LoadFromSchematic(schematic);

            Rules = _schematicDesigner.Rules;
        }

        public void LoadFromCurrentSchematic(SchematicDesignerViewModel schematicModel)
        {
            NetClassesModel.LoadFromCurrentSchematic(schematicModel);
            PartsBOMViewModel.LoadFromCurrentSchematic(schematicModel);
        }

        public void SaveToSchematic(SchematicDocument schematic)
        {
            NetClassesModel.SaveToSchematic(schematic);
        }
    }
}
