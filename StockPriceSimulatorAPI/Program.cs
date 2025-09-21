using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using Serilog;

namespace StockPriceSimulatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Replace default logger to use Serilog
            builder.Host.UseSerilog();

            var pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

            // Register services
            builder.Services.AddSingleton<PriceCalculator>();
            builder.Services.AddSingleton<StockSimulator>();
            builder.Services.AddHostedService<PriceUpdateService>();
            builder.Services.AddSignalR();

            // Services with logger
            builder.Services.AddSingleton<PluginLoader>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<PluginLoader>>();
                var env = sp.GetRequiredService<IHostEnvironment>();
                var pluginPath = Path.Combine(env.ContentRootPath, "plugins"); // safe relative path
                return new PluginLoader(pluginPath, logger);
            });
            

            // Add TCP server
            builder.Services.AddHostedService<TcpBroadcastService>();

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapHub<StockHub>("/stockhub"); // Signal IR hub

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
                        var output = formatter.FormatPrice(stock.Name, stock.CurrentPrice, DateTime.Now);

                        object finalOutput;

                        // detect JSON output by formatter name
                        if (formatter.GetType().Name.Contains("Json", StringComparison.OrdinalIgnoreCase))
                        {
                            // parse the JSON string back into a JsonElement so it’s a proper object in Swagger, so output is like in task example
                            finalOutput = JsonDocument.Parse(output).RootElement.Clone();
                        }
                        else
                        {
                            // keep plain text (CSV, etc.)
                            finalOutput = output;
                        }

                        outputs.Add(new
                        {
                            name = stock.Name,
                            formatter = formatter.GetType().Name,
                            output = finalOutput
                        });
                    }
                }

                return outputs;
            });
            app.Run();
        }     
    }

    /// <summary>
    /// Background service that updates stock prices every 5 seconds.
    /// </summary>
    public class PriceUpdateService : BackgroundService
    {
        private readonly StockSimulator _simulator;
        private readonly IHubContext<StockHub> _hub; // Signal IR hub

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
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public class TcpBroadcastService : BackgroundService
    {
        private readonly StockSimulator _simulator;
        private readonly ILogger<TcpBroadcastService> _logger;
        private readonly List<TcpClient> _clients = new();
        private TcpListener? _listener;

        public TcpBroadcastService(StockSimulator simulator, ILogger<TcpBroadcastService> logger)
        {
            _simulator = simulator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Loopback, 5002);
                _listener.Start();
                _logger.LogInformation("TCP server started on port {Port}", 5002);

                // accept clients in background
                _ = Task.Run(async () =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var client = await _listener.AcceptTcpClientAsync(stoppingToken);
                            lock (_clients) _clients.Add(client);
                            _logger.LogInformation("TCP client connected from {Address}", client.Client.RemoteEndPoint);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while accepting a TCP client");
                        }
                    }
                }, stoppingToken);

                // broadcast loop
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var snapshot = _simulator.GetAllStocks()
                            .Select(s => $"{s.Name},{s.CurrentPrice},{DateTime.Now:HH:mm:ss}")
                            .ToList();

                        var message = string.Join(Environment.NewLine, snapshot) + Environment.NewLine;
                        var data = Encoding.UTF8.GetBytes(message);

                        lock (_clients)
                        {
                            foreach (var client in _clients.ToList())
                            {
                                try
                                {
                                    client.GetStream().Write(data, 0, data.Length);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Removing disconnected client {Address}", client.Client.RemoteEndPoint);
                                    _clients.Remove(client);
                                    client.Dispose();
                                }
                            }
                        }

                        _logger.LogDebug("Broadcasted stock update to {Count} TCP clients", _clients.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during TCP broadcast loop");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            finally
            {
                _listener?.Stop();
                lock (_clients)
                {
                    foreach (var client in _clients)
                    {
                        client.Dispose();
                    }
                    _clients.Clear();
                }
                _logger.LogInformation("TCP server shut down.");
            }
        }
    }

    public class StockHub : Hub
    {
    }
}

