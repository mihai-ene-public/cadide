using System;
using System.Collections.Generic;
using System.Text;
using IDE.Core.Designers;
using IDE.Core.Storage;
using Xunit;

namespace IDE.Core.Presentation.Tests.SchematicRules
{
    public class NetsWithSinglePinSchematicRuleTests
    {
        [Theory]
        [InlineData(0, SchematicRuleResponse.NoError, true)]
        [InlineData(0, SchematicRuleResponse.Warning, true)]
        [InlineData(0, SchematicRuleResponse.Error, true)]

        [InlineData(1, SchematicRuleResponse.NoError, true)]
        [InlineData(1, SchematicRuleResponse.Warning, false)]
        [InlineData(1, SchematicRuleResponse.Error, false)]

        [InlineData(2, SchematicRuleResponse.NoError, true)]
        [InlineData(2, SchematicRuleResponse.Warning, true)]
        [InlineData(2, SchematicRuleResponse.Error, true)]
        public void CheckNet(int numberPins, SchematicRuleResponse ruleResponse, bool expectedValid)
        {
            var net = new SchematicNet();
            for (int i = 0; i < numberPins; i++)
            {
                net.NetItems.Add(new PinCanvasItem
                {
                    Number = (i + 1).ToString()
                });
            }

            var rule = new NetsWithSinglePinSchematicRule();
            rule.RuleResponse = ruleResponse;

            var result = rule.CheckNet(net);

            Assert.Equal(expectedValid, result.IsValid);
            if (!result.IsValid)
                Assert.Equal(ruleResponse, result.CheckResponse);
        }
    }
}
