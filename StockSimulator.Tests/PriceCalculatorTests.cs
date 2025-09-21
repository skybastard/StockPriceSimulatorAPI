using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockSimulator.Tests
{
    public class PriceCalculatorTests
    {
        [Theory]
        [InlineData(100, -1, 98.00)]   // -2%
        [InlineData(100, 1, 102.00)]  // +2%
        [InlineData(200, -1, 196.00)]  // -2%
        [InlineData(200, 1, 204.00)]  // +2%
        public void ApplyTwoPercentChange_ShouldReturnExpected(decimal current, int direction, decimal expected)
        {
            // Arrange
            var calc = new PriceCalculator();

            // Act
            var result = calc.ApplyTwoPercentChange(current, direction);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ApplyTwoPercentChange_InvalidDirection_Throws()
        {
            var calc = new PriceCalculator();

            Assert.Throws<ArgumentException>(() => calc.ApplyTwoPercentChange(100, 0));
        }
    }
}
