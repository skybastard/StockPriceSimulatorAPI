namespace StockSimulator.Tests
{
    public class PriceCalculator
    {
        public decimal ApplyTwoPercentChange(decimal currentPrice, int direction)
        {
            if (direction != -1 && direction != 1)
                throw new ArgumentException("Direction must be -1 or +1");

            decimal percentChange = 0.02m * direction;
            return Math.Round(currentPrice * (1 + percentChange), 2);
        }
    }
}