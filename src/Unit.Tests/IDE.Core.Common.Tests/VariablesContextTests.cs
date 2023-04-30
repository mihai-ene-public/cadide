using IDE.Core.Common.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace IDE.Core.Common.Tests;

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

    [Theory]
    [InlineData("C:/Folder/SubFolder/", "file", false)]
    [InlineData("C:/Folder/SubFolder", "file", false)]
    [InlineData("http://host-domain/folder/index.json", "http", true)]
    [InlineData("https://host/folder/index.json", "https", true)]
    public void IsIndex(string url, string scheme, bool isIndexExpected)
    {
        var uri = new Uri(url);
        Assert.True(uri.Scheme == scheme);
        var lastSegment = uri.Segments.Last();
        Assert.Equal(isIndexExpected,  "index.json".Equals(lastSegment, StringComparison.OrdinalIgnoreCase));
    }
}
