using System.Reflection;
using Contracts;

namespace StockPriceSimulatorAPI
{
    public class PluginLoader : IDisposable
    {
        private readonly List<IDataFormatter> _formatters = new();

        public PluginLoader(string pluginPath, ILogger<PluginLoader> logger)
        {
            if (!Directory.Exists(pluginPath))
            {
                logger.LogWarning("Plugin folder '{Path}' not found.", pluginPath);
                return;
            }

            foreach (var dll in Directory.GetFiles(pluginPath, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    foreach (var type in assembly.GetTypes()
                        .Where(t => typeof(IDataFormatter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
                    {
                        if (Activator.CreateInstance(type) is IDataFormatter formatter)
                        {
                            _formatters.Add(formatter);
                            logger.LogInformation("Loaded plugin {Plugin} from {Dll}", type.FullName, dll);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load plugin from {Dll}", dll);
                }
            }
        }

        public IEnumerable<IDataFormatter> GetFormatters() => _formatters;

        public void Dispose()
        {
            foreach (var f in _formatters)
            {
                if (f is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
