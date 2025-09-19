
namespace StockPriceSimulatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register StockSimulator as singleton
            builder.Services.AddSingleton<StockSimulator>();

            // Background service for price updates
            builder.Services.AddHostedService<PriceUpdateService>();

            // Add Swagger services to try the REST endpoints
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseHttpsRedirection();

            // Enable Swagger middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // GET /prices -> all current prices
            app.MapGet("/prices", (StockSimulator simulator) =>
            {
                return simulator.GetAllStocks()
                    .Select(stock => new
                    {
                        stock.Name,
                        stock.CurrentPrice
                    });
            });

            // GET /prices/{symbol} -> current + history
            app.MapGet("/prices/{symbol}", (string symbol, StockSimulator simulator) =>
            {
                try
                {
                    var stock = simulator.GetStock(symbol);
                    return Results.Ok(new
                    {
                        stock.Name,
                        stock.CurrentPrice,
                        History = stock.GetHistory()
                            .Select(h => new { h.Timestamp, h.Price })
                    });
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }
            });

            app.Run();
        }


        /// <summary>
        /// Background service that updates stock prices every 5 seconds.
        /// </summary>
        public class PriceUpdateService : BackgroundService
        {
            private readonly StockSimulator _simulator;

            public PriceUpdateService(StockSimulator simulator)
            {
                _simulator = simulator;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                        _simulator.UpdatePrices();

                    await Console.Out.WriteLineAsync();
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}

