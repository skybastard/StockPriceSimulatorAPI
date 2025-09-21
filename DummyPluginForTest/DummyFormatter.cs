using Contracts;
using System;

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
