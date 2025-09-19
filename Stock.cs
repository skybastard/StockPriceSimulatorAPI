using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceSimulator
{
    public class Stock
    {
        public string Name { get; }
        public decimal CurrentPrice { get; private set; }
        private readonly Queue<(decimal, DateTime Timestamp)> PriceHistory;

        public Stock(string name, decimal currentPrice)
        {
            Name = name;
            CurrentPrice = currentPrice;
            PriceHistory = new Queue<(decimal, DateTime Timestamp)>();
        }

        public void UpdatePrice(decimal newPrice)
        {
            CurrentPrice = newPrice;
            AddToHistory(newPrice);

        }

        private void AddToHistory(decimal price)
        {
            if (PriceHistory.Count >= 10)
                PriceHistory.Dequeue();

            PriceHistory.Enqueue((Math.Round(price, 2), DateTime.Now));
        }

        public IReadOnlyCollection<(decimal Price, DateTime Timestamp)> GetHistory() => PriceHistory;
        

    }
}
