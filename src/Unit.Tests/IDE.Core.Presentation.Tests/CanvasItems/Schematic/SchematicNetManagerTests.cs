using IDE.Core.Designers;
using IDE.Core.ViewModels;
using Xunit;

namespace IDE.Core.Presentation.Tests
{
    public class SchematicNetManagerTests
    {

        //todo: test for assert classid

        [Fact]
        public void AddNet_SingleNet()
        {
            var nm = new SchematicNetManager();

            var netName = "GND";
            var net = nm.Add(netName);

            Assert.NotNull(net);
            Assert.Equal(netName, net.Name);
            Assert.Equal(1, nm.Elements.Count);
            Assert.NotNull(net.Id);
            Assert.True(net.Id?.Length > 0);
        }

        [Fact]
        public void AddNet_SameNetName()
        {
            var nm = new SchematicNetManager();

            var netName = "GND";
            nm.Add(netName);

            var net = nm.Add(netName);

            Assert.NotNull(net);
            Assert.Equal(netName, net.Name);
            Assert.Equal(1, nm.Elements.Count);
        }

        [Fact]
        public void AddNet_NewerNetId()
        {
            var nm = new SchematicNetManager();

            var gndNet1 = new SchematicNet
            {
                Id = "1",
                Name = "GND"
            };
            nm.Add(gndNet1);

            var gndNet2 = new SchematicNet
            {
                Id = "1",
                Name = "GND"
            };

            var net = nm.Add(gndNet2);

            Assert.NotNull(net);
            Assert.Equal(1, nm.Elements.Count);
            Assert.Equal(gndNet1, net);
        }

        [Fact]
        public void AddNet_OlderNetId()
        {
            var nm = new SchematicNetManager();

            var gndNet1 = new SchematicNet
            {
                Id = "1",
                Name = "GND"
            };
            nm.Add(gndNet1);

            var gndNet2 = new SchematicNet
            {
                Id = "1",
                Name = "GND"
            };

            var net = nm.Add(gndNet2);

            Assert.NotNull(net);
            Assert.Equal(1, nm.Elements.Count);
            Assert.Equal(gndNet2, net);//?
        }
    }
}
