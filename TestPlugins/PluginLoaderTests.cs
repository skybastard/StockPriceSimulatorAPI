using Microsoft.Extensions.Logging.Abstractions;
using StockPriceSimulatorAPI;

namespace TestPlugins
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadsAndExecutesDummyPlugin()
        {
            // Arrange
            var pluginPath = PreparePluginFolder();
            var loader = new PluginLoader(pluginPath, NullLogger<PluginLoader>.Instance);

            // Act
            var formatter = loader.GetFormatters().FirstOrDefault();

            // Assert
            Assert.NotNull(formatter);
            var result = formatter!.FormatPrice("AAPL", 123.45m, new DateTime(2025, 1, 1, 10, 0, 0));
            Assert.StartsWith("DUMMY-AAPL-123.45", result);
        }
        private string PreparePluginFolder()
        {
            var pluginPath = Path.Combine(Path.GetTempPath(), "plugins");
            if (Directory.Exists(pluginPath))
                Directory.Delete(pluginPath, true);

            Directory.CreateDirectory(pluginPath);

            // Copy compiled plugin dll into test folder
            var sourceDll = Path.Combine(
                AppContext.BaseDirectory,
                "DummyPluginForTest.dll"); // adjust path depending on build layout
            var destDll = Path.Combine(pluginPath, "DummyPluginForTest.dll");
            File.Copy(sourceDll, destDll, true);

            return pluginPath;
        }

    }
}