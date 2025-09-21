using StockPriceSimulator;

namespace StockPriceSimulatorAPI
{
    public class StockSimulator
    {
        private readonly PriceCalculator _calculator;
        private readonly Random _random = new Random();
        private readonly Dictionary<string, Stock> _stocks;


        public StockSimulator(PriceCalculator calculator)
        {
            _calculator = calculator;
            _stocks = new Dictionary<string, Stock>();

            _stocks["AAPL"] = new Stock("AAPL", 238.99m );
            _stocks["MSFT"] = new Stock("MSFT", 510.02m );
            _stocks["GOOGL"] = new Stock("GOOGL", 249.03m );
            _stocks["TSLA"] = new Stock("TSLA", 425.86m );
            _stocks["AMZN"] = new Stock("AMZN", 231.86m );
        }

        public void UpdatePrices()
        {
            //foreach (var stock in _stocks.Values)
            //{
                //decimal newPrice = NextPrice(stock.CurrentPrice);
                //stock.UpdatePrice(newPrice);
                //Console.WriteLine($"{stock.Name} => {stock.CurrentPrice} history: {string.Join(", ", stock.GetHistory().Select(h => h.Price))}"); ;
                foreach (var stock in _stocks.Values)
                {
                    var newPrice = _calculator.ApplyRandomChange(stock.CurrentPrice);
                    stock.UpdatePrice(newPrice);
                }
            //}
        }

        //private decimal NextPrice(decimal currentPrice)
        //{
        //    int direction = _random.Next(2) == 0 ? -1 : 1;
        //    decimal percentChange = 0.02m * direction;
        //    return Math.Round(currentPrice * (1 + percentChange), 2);
        //}

        public Stock GetStock(string name) => _stocks[name];
        public IEnumerable<Stock> GetAllStocks() => _stocks.Values;


    }
}
