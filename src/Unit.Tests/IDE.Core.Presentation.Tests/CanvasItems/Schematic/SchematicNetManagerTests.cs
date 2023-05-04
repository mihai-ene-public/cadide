using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using Moq;
using Xunit;

namespace IDE.Core.Presentation.Tests;

public class SchematicNetManagerTests
{
    public SchematicNetManagerTests()
    {
        var schDoc = new Mock<ISchematicDesigner>();
        _netManager = new SchematicNetManager(schDoc.Object);
    }
    private readonly SchematicNetManager _netManager;

    [Fact]
    public void AddNew()
    {
        var net = _netManager.AddNew();

        Assert.NotNull(net);
        Assert.NotNull(net.Name);
        Assert.NotNull(net.Id);
        Assert.StartsWith("Net$", net.Name);
    }

    [Fact]
    public void AddNet_SingleNet()
    {
        var netName = "GND";
        var net = _netManager.Add(netName);

        Assert.NotNull(net);
        Assert.Equal(netName, net.Name);
        Assert.Equal(1, _netManager.Elements.Count);
        Assert.NotNull(net.Id);
        Assert.True(net.Id?.Length > 0);
    }

    [Fact]
    public void AddNet_SameNetName()
    {
        var netName = "GND";
        _netManager.Add(netName);

        var net = _netManager.Add(netName);

        Assert.NotNull(net);
        Assert.Equal(netName, net.Name);
        Assert.Equal(1, _netManager.Elements.Count);
    }

    [Fact]
    public void AddNet_NewerNetId()
    {
        var gndNet1 = new SchematicNet
        {
            Id = "1",
            Name = "GND"
        };
        _netManager.Add(gndNet1);

        var gndNet2 = new SchematicNet
        {
            Id = "1",
            Name = "GND"
        };

        var net = _netManager.Add(gndNet2);

        Assert.NotNull(net);
        Assert.Equal(1, _netManager.Elements.Count);
        Assert.Equal(gndNet1, net);
    }

    [Fact]
    public void Remove()
    {
        var net = _netManager.AddNew();

        _netManager.Remove(net);

        Assert.Equal(0, _netManager.Elements.Count);
    }

    [Fact]
    public void Get()
    {
        var net = _netManager.AddNew();

        var existing = _netManager.Get(net.Name);

        Assert.NotNull(net);
        Assert.NotNull(existing);
        Assert.Equal(net, existing);
    }
}
