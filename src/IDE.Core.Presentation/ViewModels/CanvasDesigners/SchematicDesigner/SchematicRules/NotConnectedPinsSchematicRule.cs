using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Designers
{
    /* Takes a pin and checks if it belongs to a net
     */
    public class NotConnectedPinsSchematicRule : SingleItemSchematicRule
    {
        public string Description => "Not connected pins";

        public override SchematicRuleCheckResult CheckItem(ISelectableItem pin)
        {
            var res = new SchematicRuleCheckResult { IsValid = true };

            if (RuleResponse == SchematicRuleResponse.NoError)
                return res;

            if (pin is PinCanvasItem p)
            {
                if (p.Net == null)
                {
                    res.IsValid = false;
                    res.Message = $"Pin {p.Name} is not connected";
                    res.CheckResponse = RuleResponse;
                }
            }
            return res;
        }

        public override void LoadFromData(ISchematicRuleData rule)
        {
            var r = rule as NotConnectedPinsSchematicRuleData;

            IsEnabled = r.IsEnabled;
            RuleResponse = r.RuleResponse;
        }

        public override SchematicRuleData SaveToSchematicRule()
        {
            return new NotConnectedPinsSchematicRuleData
            {
                IsEnabled = IsEnabled,
                RuleResponse = RuleResponse
            };
        }
    }
}
