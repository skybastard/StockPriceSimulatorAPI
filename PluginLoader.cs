using System.Reflection;

namespace StockPriceSimulatorAPI
{
    public class PluginLoader
    {
        private readonly List<IDataFormatter> _formatters = new();

        public PluginLoader(string pluginPath)
        {
            foreach (var dll in Directory.GetFiles(pluginPath, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(dll);

                var types = assembly.GetTypes()
                    .Where(t => typeof(IDataFormatter).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in types)
                {
                    if (Activator.CreateInstance(type) is IDataFormatter formatter)
                    {
                        _formatters.Add(formatter);
                        Console.WriteLine($"Loaded plugin: {type.FullName}");
                    }
                }
            }
        }

        public IEnumerable<IDataFormatter> GetFormatters() => _formatters;
    }
}
