using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.GenreManager
{
    /// <summary>
    /// Registers genre sections with HomeScreen plugin at startup.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <inheritdoc />
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            // Add a hosted service that will register our sections after HomeScreen is ready
            serviceCollection.AddHostedService<GenreRegistrationHostedService>();
        }
    }
}
