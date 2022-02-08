using IDE.Core.Common.Variables;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IDE.Core.Common.Tests
{
    public class VariablesContextTests
    {

        [Theory]
        [InlineData("projectName", "Project example", "{projectName}", "Project example")]
        [InlineData("projectName", "Project example", "{projectName}-{extra}", "Project example-extra")]
        public void ReplaceReturnsExpected(string variableName, string varValue, string inputText, string expectedOutput)
        {
            var varContext = new VariablesContext();
            varContext.Add(new Variable(variableName, varValue));
            varContext.Add(new Variable("extra", "extra"));

            var actual = varContext.Replace(inputText);

            Assert.Equal(expectedOutput, actual);
        }
    }
}
