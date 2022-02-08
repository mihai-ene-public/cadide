using System;
using Xunit;
using IDE.Core.Storage;
using IDE.Core.Interfaces;

namespace IDE.Core.Data.Tests
{
    public class LayerConstantsTest
    {
        [Theory]
        //top copper
        [InlineData(LayerConstants.SignalTopLayerId, FootprintPlacement.Top, LayerConstants.SignalTopLayerId)]
        [InlineData(LayerConstants.SignalTopLayerId, FootprintPlacement.Bottom, LayerConstants.SignalBottomLayerId)]
        //top paste
        [InlineData(LayerConstants.PasteTopLayerId, FootprintPlacement.Top, LayerConstants.PasteTopLayerId)]
        [InlineData(LayerConstants.PasteTopLayerId, FootprintPlacement.Bottom, LayerConstants.PasteBottomLayerId)]
        //top solder mask
        [InlineData(LayerConstants.SolderTopLayerId, FootprintPlacement.Top, LayerConstants.SolderTopLayerId)]
        [InlineData(LayerConstants.SolderTopLayerId, FootprintPlacement.Bottom, LayerConstants.SolderBottomLayerId)]
        //top silkscreen
        [InlineData(LayerConstants.SilkscreenTopLayerId, FootprintPlacement.Top, LayerConstants.SilkscreenTopLayerId)]
        [InlineData(LayerConstants.SilkscreenTopLayerId, FootprintPlacement.Bottom, LayerConstants.SilkscreenBottomLayerId)]
        //top mechanical
        [InlineData(LayerConstants.MechanicalTopLayerId, FootprintPlacement.Top, LayerConstants.MechanicalTopLayerId)]
        [InlineData(LayerConstants.MechanicalTopLayerId, FootprintPlacement.Bottom, LayerConstants.MechanicalBottomLayerId)]

        //bottom copper
        [InlineData(LayerConstants.SignalBottomLayerId, FootprintPlacement.Top, LayerConstants.SignalTopLayerId)]
        [InlineData(LayerConstants.SignalBottomLayerId, FootprintPlacement.Bottom, LayerConstants.SignalBottomLayerId)]
        //bottom paste
        [InlineData(LayerConstants.PasteBottomLayerId, FootprintPlacement.Top, LayerConstants.PasteTopLayerId)]
        [InlineData(LayerConstants.PasteBottomLayerId, FootprintPlacement.Bottom, LayerConstants.PasteBottomLayerId)]
        //bottom solder mask
        [InlineData(LayerConstants.SolderBottomLayerId, FootprintPlacement.Top, LayerConstants.SolderTopLayerId)]
        [InlineData(LayerConstants.SolderBottomLayerId, FootprintPlacement.Bottom, LayerConstants.SolderBottomLayerId)]
        //bottom silkscreen
        [InlineData(LayerConstants.SilkscreenBottomLayerId, FootprintPlacement.Top, LayerConstants.SilkscreenTopLayerId)]
        [InlineData(LayerConstants.SilkscreenBottomLayerId, FootprintPlacement.Bottom, LayerConstants.SilkscreenBottomLayerId)]
        //bottom mech
        [InlineData(LayerConstants.MechanicalBottomLayerId, FootprintPlacement.Top, LayerConstants.MechanicalTopLayerId)]
        [InlineData(LayerConstants.MechanicalBottomLayerId, FootprintPlacement.Bottom, LayerConstants.MechanicalBottomLayerId)]
        public void GetPairedLayer_ShouldCalculate(int layerId, FootprintPlacement placement, int expectedLayerId)
        {
            var actual = LayerConstants.GetPairedLayer(layerId, placement);

            Assert.Equal(expectedLayerId, actual);
        }

        [Theory]
        [InlineData(LayerConstants.SignalTopLayerId, LayerType.SilkScreen, LayerConstants.SilkscreenTopLayerId)]
        [InlineData(LayerConstants.SignalBottomLayerId, LayerType.SilkScreen, LayerConstants.SilkscreenBottomLayerId)]

        [InlineData(LayerConstants.SignalTopLayerId, LayerType.Mechanical, LayerConstants.MechanicalTopLayerId)]
        [InlineData(LayerConstants.SignalBottomLayerId, LayerType.Mechanical, LayerConstants.MechanicalBottomLayerId)]

        [InlineData(LayerConstants.SignalTopLayerId, LayerType.PasteMask, LayerConstants.PasteTopLayerId)]
        [InlineData(LayerConstants.SignalBottomLayerId, LayerType.PasteMask, LayerConstants.PasteBottomLayerId)]

        [InlineData(LayerConstants.SignalTopLayerId, LayerType.SolderMask, LayerConstants.SolderTopLayerId)]
        [InlineData(LayerConstants.SignalBottomLayerId, LayerType.SolderMask, LayerConstants.SolderBottomLayerId)]
        public void GetCompanionLayer_ShouldReturnValue(int layerId, LayerType layerType, int expected)
        {
            var actual = LayerConstants.GetCompanionLayer(layerId, layerType);

            Assert.Equal(expected, actual);
        }
    }
}
