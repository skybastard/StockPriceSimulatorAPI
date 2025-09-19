using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace StockPriceSimulator
{
    public static class PriceLogic
    {
        private static readonly Random random = new Random();
        public static decimal ChangePrice(decimal currentPrice)
        {
            int direction = random.Next(2) == 0 ? -1 : 1;
            decimal percentChange = (decimal)(1 + (0.02 * direction)); // ends up either 1,02 or 0,98
            decimal newPrice = currentPrice * percentChange;           
            return newPrice;
        }


        

    

    }
}
