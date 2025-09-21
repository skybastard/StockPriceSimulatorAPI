using Microsoft.Extensions.Logging.Abstractions;
using StockPriceSimulatorAPI;

namespace TestPlugins
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadsCsvPluginAndFormatsPrice()
        {
            var pluginPath = PreparePluginFolderWithRealCsvPlugin();
            var loader = new PluginLoader(pluginPath, NullLogger<PluginLoader>.Instance);

            var formatter = loader.GetFormatters().FirstOrDefault(f => f.GetType().Name.Contains("Csv"));
            Assert.NotNull(formatter);

            var result = formatter!.FormatPrice("AAPL", 123.45m, new DateTime(2025, 1, 1, 10, 0, 0));
            Assert.Contains("AAPL,123.45", result);
        }
        private string PreparePluginFolderWithRealCsvPlugin()
        {
            // Create a clean plugins folder under the test output
            var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (Directory.Exists(pluginPath))
                Directory.Delete(pluginPath, true);

            Directory.CreateDirectory(pluginPath);

            // Locate the built CsvFormatter.dll (adjust path if project/target changes)
            var sourceDll = Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",   // back to solution root
                "CsvFormatter",
                "bin", "Debug", "net8.0",
                "CsvFormatter.dll");

            if (!File.Exists(sourceDll))
                throw new FileNotFoundException("CsvFormatter.dll not found", sourceDll);

            // Copy it into the test's plugins folder
            var destDll = Path.Combine(pluginPath, "CsvFormatter.dll");
            File.Copy(sourceDll, destDll, true);

            return pluginPath;
        }
    }
}