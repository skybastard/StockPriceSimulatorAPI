using Contracts;

namespace DummyPluginForTest
{
    public class DummyFormatter : IDataFormatter
    {
        public string FormatPrice(string name, decimal price, DateTime timestamp)
        {
            return $"DUMMY-{name}-{price}-{timestamp:HH:mm:ss}";
        }
    }
}
