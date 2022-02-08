using IDE.Core.Converters;
using System;
using Xunit;

namespace IDE.Core.Types.Tests
{
    public class MilimetersToDpiHelperTests
    {
        [Theory]
        [InlineData(25.4, 96.0)]
        [InlineData(0.0, 0.0)]
        public void ConvertToDpiShouldReturnCorrectValue(double mmValue, double expectedDpi)
        {
            var actual = MilimetersToDpiHelper.ConvertToDpi(mmValue);

            Assert.Equal(expectedDpi, actual);
        }

        [Theory]
        [InlineData(96.0, 25.4)]
        [InlineData(0.0, 0.0)]
        public void ConvertToMMShouldReturnCorrectValue(double dpiValue, double expectedMM)
        {
            var actual = MilimetersToDpiHelper.ConvertToMM(dpiValue);

            Assert.Equal(expectedMM, actual);
        }
    }
}
