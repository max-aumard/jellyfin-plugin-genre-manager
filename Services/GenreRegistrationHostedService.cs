using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.GenreManager.HomeScreen.Sections;
using Jellyfin.Plugin.GenreManager.Library;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.GenreManager
{
    /// <summary>
    /// Hosted service that registers genre sections with HomeScreen plugin.
    /// </summary>
    public class GenreRegistrationHostedService : IHostedService
    {
        private readonly ILogger<GenreRegistrationHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreRegistrationHostedService"/> class.
        /// </summary>
        public GenreRegistrationHostedService(
            ILogger<GenreRegistrationHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run registration in background after a delay to ensure HomeScreen is loaded
            _ = Task.Run(async () =>
            {
                try
                {
                    // Wait 5 seconds for HomeScreen to initialize
                    await Task.Delay(5000, cancellationToken);

                    _logger.LogInformation("[Genre Manager] Attempting to register genre sections with HomeScreen");

                    // Try to get IHomeScreenManager from DI
                    var homeScreenManager = _serviceProvider.GetService(typeof(IHomeScreenManager)) as IHomeScreenManager;

                    if (homeScreenManager == null)
                    {
                        _logger.LogWarning("[Genre Manager] IHomeScreenManager not found in DI - HomeScreen plugin may not be installed");
                        return;
                    }

                    _logger.LogInformation("[Genre Manager] Found IHomeScreenManager, registering genre sections");

                    // Get all genre section types from our assembly
                    var genreSectionTypes = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && t.BaseType == typeof(GenreSectionBase))
                        .ToList();

                    _logger.LogInformation("[Genre Manager] Found {Count} genre section types", genreSectionTypes.Count);

                    int registeredCount = 0;
                    foreach (var sectionType in genreSectionTypes)
                    {
                        try
                        {
                            // Create instance using DI
                            var instance = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(_serviceProvider, sectionType) as IHomeScreenSection;

                            if (instance != null)
                            {
                                // Use reflection to call RegisterResultsDelegate<T>(T handler)
                                var registerMethod = homeScreenManager.GetType()
                                    .GetMethods()
                                    .FirstOrDefault(m =>
                                        m.Name == "RegisterResultsDelegate" &&
                                        m.IsGenericMethod == false &&
                                        m.GetParameters().Length == 1);

                                if (registerMethod != null)
                                {
                                    // Make it generic with our section type
                                    var genericMethod = registerMethod.MakeGenericMethod(sectionType);
                                    genericMethod.Invoke(homeScreenManager, new object[] { instance });

                                    registeredCount++;
                                    _logger.LogInformation("[Genre Manager] Registered section: {Section}", instance.Section);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[Genre Manager] Error registering section type: {Type}", sectionType.Name);
                        }
                    }

                    _logger.LogInformation("[Genre Manager] Successfully registered {Count} genre sections with HomeScreen", registeredCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Genre Manager] Error during genre section registration");
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
