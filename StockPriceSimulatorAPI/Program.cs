using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi;

namespace StockPriceSimulatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
         
            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

            


            // Register services
            builder.Services.AddSingleton<StockSimulator>();
            builder.Services.AddHostedService<PriceUpdateService>();
            builder.Services.AddSingleton(new PluginLoader(pluginPath));
            builder.Services.AddSignalR();

            

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapHub<StockHub>("/stockhub"); // ir hub

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

            app.MapGet("/formatted-prices", (StockSimulator simulator, PluginLoader loader) =>
            {
                var outputs = new List<object>();

                foreach (var stock in simulator.GetAllStocks())
                {
                    foreach (var formatter in loader.GetFormatters())
                    {
                        outputs.Add(new
                        {
                            stock.Name,
                            Formatter = formatter.GetType().Name,
                            Output = formatter.FormatPrice(stock.Name, stock.CurrentPrice, DateTime.Now)
                        });
                    }
                }

                return outputs;
            });


            app.Run();
        }


        /// <summary>
        /// Background service that updates stock prices every 5 seconds.
        /// </summary>
        public class PriceUpdateService : BackgroundService
        {
            private readonly StockSimulator _simulator;
            private readonly IHubContext<StockHub> _hub; // ir hub

            public PriceUpdateService(StockSimulator simulator, IHubContext<StockHub> hub)
            {
                _simulator = simulator;
                _hub = hub;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _simulator.UpdatePrices();

                    // broadcast to all connected IR clients
                    await _hub.Clients.All.SendAsync("ReceivePrices",
                        _simulator.GetAllStocks().Select(s => new {
                            s.Name,
                            s.CurrentPrice
                        }), stoppingToken);

                    await Console.Out.WriteLineAsync();
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }
    }

    public class StockHub : Hub
    {
    }

    public class StockUpdate
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
    }
}

