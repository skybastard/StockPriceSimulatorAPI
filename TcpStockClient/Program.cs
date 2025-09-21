using System.Net.Sockets;
using System.Text;

namespace TcpStockClient
{
    internal class Program
    {
        static async Task Main()
        {
            while (true)
            {
                try
                {
                    using var client = new TcpClient();
                    Console.WriteLine("Trying to connect to TCP server...");

                    await client.ConnectAsync("localhost", 5002);
                    Console.WriteLine("Connected to TCP server");

                    using var stream = client.GetStream();
                    var buffer = new byte[4096];

                    while (true)
                    {
                        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("Server closed connection, retrying...");
                            break;
                        }

                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Update for TCP client:");
                        Console.WriteLine(message);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("TCP server not available, retrying in 3s...");
                    await Task.Delay(3000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}, retrying in 3s...");
                    await Task.Delay(3000);
                }
            }
        }
    }
}
