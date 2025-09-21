using Contracts;
using System.Text.Json;

namespace JsonFormatter
{
    public class JsonPriceFormatter : IDataFormatter
    {
        public string FormatPrice(string symbol, decimal price, DateTime timestamp)
        {
            return JsonSerializer.Serialize(new { symbol, price, timestamp });
        }
    }
}
