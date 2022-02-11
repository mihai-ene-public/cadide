using IDE.Core.Common.Utilities;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;

namespace IDE.Core.ViewModels
{
    public class SchematicRulesDataToModelMapper : GenericMapper, ISchematicRulesToModelMapper
    {

        public SchematicRulesDataToModelMapper() : base()
        {
        }

        protected override void CreateMappings()
        {
            AddMapping(typeof(PinTypesConnectionSchematicRuleData), typeof(PinTypesConnectionSchematicRule));
            AddMapping(typeof(NetsWithSinglePinSchematicRuleData), typeof(NetsWithSinglePinSchematicRule));
            AddMapping(typeof(NotConnectedPinsSchematicRuleData), typeof(NotConnectedPinsSchematicRule));
        }



        public ISchematicRuleModel CreateRuleItem(ISchematicRuleData rule)
        {
            var mappedType = GetMapping(rule.GetType());
            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as ISchematicRuleModel;

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
