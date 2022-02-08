using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Linq;

namespace IDE.Core.Designers
{
    /* Takes a net and checks the number of pins
     */
    public class NetsWithSinglePinSchematicRule : NetSchematicRule
    {
        public string Description => "Nets with single pin";


        public override SchematicRuleCheckResult CheckNet(SchematicNet net)
        {
            var res = new SchematicRuleCheckResult { IsValid = true };

            if (RuleResponse == SchematicRuleResponse.NoError)
                return res;

            var pins = net.NetItems.OfType<PinCanvasItem>().ToList();

            if (pins.Count == 1)
            {
                res.IsValid = false;
                res.Message = $"Net {net.Name} has one single pin";
                res.CheckResponse = RuleResponse;
            }

            return res;
        }

        public override void LoadFromData(ISchematicRuleData rule)
        {
            var r = rule as NetsWithSinglePinSchematicRuleData;

            IsEnabled = r.IsEnabled;
            RuleResponse = r.RuleResponse;
        }

        public override SchematicRuleData SaveToSchematicRule()
        {
            return new NetsWithSinglePinSchematicRuleData
            {
                IsEnabled = IsEnabled,
                RuleResponse = RuleResponse
            };
        }
    }
}
