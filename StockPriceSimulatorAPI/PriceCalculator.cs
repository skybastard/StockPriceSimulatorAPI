namespace StockPriceSimulatorAPI
{
    public class PriceCalculator
    {
        
        private readonly Random _random = new();

        /// <summary>
        /// Applies a random price change of -2% to +2%.
        /// </summary>
        public decimal ApplyRandomChange(decimal currentPrice)
        {
            int direction = _random.Next(2) == 0 ? -1 : 1;
            decimal percentChange = 0.02m * direction;
            return Math.Round(currentPrice * (1 + percentChange), 2);
        }
    }
}

