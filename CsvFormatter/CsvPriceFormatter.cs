using Contracts;

namespace CsvFormatter
{
    public class CsvPriceFormatter : IDataFormatter
    {
        public string FormatPrice(string symbol, decimal price, DateTime timestamp)
        {
            return $"{symbol},{price},{timestamp:HH:mm:ss}";
        }
    }
}
