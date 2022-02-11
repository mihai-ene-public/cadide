using IDE.Core.Common.Utilities;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;

namespace IDE.Core.ViewModels
{
    public class BoardRulesDataToModelMapper : GenericMapper, IBoardRulesDataToModelMapper
    {

        public BoardRulesDataToModelMapper() : base()
        {
        }

        protected override void CreateMappings()
        {
            AddMapping(typeof(ElectricalClearanceRule), typeof(ElectricalClearanceRuleModel));
            AddMapping(typeof(TrackWidthRule), typeof(TrackWidthRuleModel));
            AddMapping(typeof(ViaDefinitionRule), typeof(ViaDefinitionRuleModel));
            AddMapping(typeof(MaskExpansionRule), typeof(MaskExpansionRuleModel));
            AddMapping(typeof(ManufacturingHoleSizeRule), typeof(ManufacturingHoleSizeRuleModel));
            AddMapping(typeof(ManufacturingClearanceRule), typeof(ManufacturingClearanceRuleModel));
            AddMapping(typeof(GroupRule), typeof(GroupRuleModel));
        }

       

        public IBoardRuleModel CreateRuleItem(IBoardRuleData rule)
        {
            var mappedType = GetMapping(rule.GetType());
            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as IBoardRuleModel;

                if (nodeModel != null)
                {
                    nodeModel.LoadFromData(rule);
                    return nodeModel;
                }
            }

            return null;
        }
    }
}
