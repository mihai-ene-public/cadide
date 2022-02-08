using IDE.Core.Designers;
using IDE.Core.Storage;
using Xunit;

namespace IDE.Core.Presentation.Tests.SchematicRules
{
    public class NotConnectedPinsSchematicRuleTests
    {
        [Theory]
        [InlineData(true, SchematicRuleResponse.NoError, SchematicRuleResponse.NoError, true)]
        [InlineData(true, SchematicRuleResponse.Warning, SchematicRuleResponse.NoError, true)]
        [InlineData(true, SchematicRuleResponse.Error, SchematicRuleResponse.NoError, true)]

        [InlineData(false, SchematicRuleResponse.NoError, SchematicRuleResponse.NoError, true)]
        [InlineData(false, SchematicRuleResponse.Warning, SchematicRuleResponse.Warning, false)]
        [InlineData(false, SchematicRuleResponse.Error, SchematicRuleResponse.Error, false)]
        public void CheckItem(bool netIsSetToPin, SchematicRuleResponse setdRuleResponse, SchematicRuleResponse expectedRuleResponse, bool expectedValid)
        {
            var pin = new PinCanvasItem();
            if (netIsSetToPin)
            {
                var net = new SchematicNet();
                pin.AssignNet(net);
            }

            var rule = new NotConnectedPinsSchematicRule();
            rule.RuleResponse = setdRuleResponse;

            var result = rule.CheckItem(pin);

            Assert.Equal(expectedValid, result.IsValid);
            Assert.Equal(expectedRuleResponse, result.CheckResponse);
        }
    }
}
