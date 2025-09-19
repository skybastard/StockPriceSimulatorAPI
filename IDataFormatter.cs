﻿namespace StockPriceSimulatorAPI
{
    public interface IDataFormatter
    {
        string FormatPrice(string symbol, decimal price, DateTime timestamp);
    }
}
