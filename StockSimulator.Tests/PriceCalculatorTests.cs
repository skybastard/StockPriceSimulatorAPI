using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPriceSimulatorAPI;


namespace StockPriceTests
{
    public class PriceCalculatorTests
    {
        [Fact]
        public void UpdatePrices_ChangesWithinExpectedRange()
        {
            var calc = new PriceCalculator();
            var sim = new StockSimulator(calc);

            var stock = sim.GetStock("AAPL")!;
            var original = stock.CurrentPrice;

            sim.UpdatePrices();

            var updated = stock.CurrentPrice;
            var percentChange = (updated - original) / original * 100;

            Assert.InRange(percentChange, -2.01m, 2.01m);
        }
    }
}
