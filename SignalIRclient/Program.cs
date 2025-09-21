using Microsoft.AspNetCore.SignalR.Client;

namespace SignalIRclient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7006/stockhub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<IEnumerable<StockUpdate>>("ReceivePrices", prices =>
            {
                Console.WriteLine();
                Console.WriteLine("Stock updates:");
                foreach (var stock in prices)
                {
                    Console.WriteLine($"{stock.Name}: {stock.CurrentPrice}");
                }
            });

            
            while (true) // dont start before the hub is up and running
            {
                try
                {
                    await connection.StartAsync();
                    Console.WriteLine("Connected to SignalR hub.");
                    break; // exit loop if successful
                }
                catch
                {
                    Console.WriteLine("Hub not ready, retrying in 3s...");
                    await Task.Delay(3000);
                }
            }

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }


    //Define a DTO to client to match exactly:
    public class StockUpdate
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
    }
}
