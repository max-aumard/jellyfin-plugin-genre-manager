using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.GenreManager.Services
{
    /// <summary>
    /// Startup service that registers genre sections with Home Screen Sections plugin.
    /// </summary>
    public class GenreStartupService : IScheduledTask
    {
        private readonly ILogger<GenreStartupService> _logger;
        private readonly IServerApplicationHost _serverApplicationHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreStartupService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serverApplicationHost">The server application host.</param>
        public GenreStartupService(
            ILogger<GenreStartupService> logger,
            IServerApplicationHost serverApplicationHost)
        {
            _logger = logger;
            _serverApplicationHost = serverApplicationHost;
        }

        /// <inheritdoc />
        public string Name => "Genre Manager Startup";

        /// <inheritdoc />
        public string Key => "GenreManagerStartup";

        /// <inheritdoc />
        public string Description => "Registers genre sections with Home Screen Sections plugin";

        /// <inheritdoc />
        public string Category => "Genre Manager";

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Genre Manager] Starting up, registering genre sections");

            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                _logger.LogError("[Genre Manager] Configuration not found");
                return Task.CompletedTask;
            }

            // Wait a bit for Home Screen Sections plugin to be ready
            _logger.LogInformation("[Genre Manager] Waiting 10 seconds for Home Screen Sections to be ready...");
            Task.Delay(10000, cancellationToken).Wait(cancellationToken);

            RegisterGenreSections();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo
            {
                Type = TaskTriggerInfo.TriggerStartup
            };
        }

        private void RegisterGenreSections()
        {
            try
            {
                var config = Plugin.Instance?.Configuration;
                if (config == null || config.SelectedGenres == null || !config.SelectedGenres.Any())
                {
                    _logger.LogWarning("[Genre Manager] No genres configured");
                    return;
                }

                // Get the Jellyfin server URL
                string baseUrl = $"http://localhost:{_serverApplicationHost.HttpPort}";

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    int registeredCount = 0;

                    // Remove duplicates from selected genres
                    var uniqueGenres = config.SelectedGenres.Distinct().ToList();
                    _logger.LogInformation("[Genre Manager] Registering {Count} unique genres", uniqueGenres.Count);

                    foreach (var genre in uniqueGenres)
                    {
                        try
                        {
                            RegisterGenreSection(client, genre);
                            registeredCount++;
                            _logger.LogInformation("[Genre Manager] Registered section for genre: {Genre}", genre);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[Genre Manager] Error registering genre section: {Genre}", genre);
                        }
                    }

                    _logger.LogInformation("[Genre Manager] Successfully registered {Count} genre sections", registeredCount);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Genre Manager] Error registering genre sections");
            }
        }

        private void RegisterGenreSection(HttpClient client, string genre)
        {
            // Create the registration payload
            var sectionId = $"Genre_{genre.Replace(" ", "")}";
            var resultsEndpoint = $"/GenreSection/{Uri.EscapeDataString(genre)}";

            var registerPayload = new JObject
            {
                { "id", sectionId },
                { "displayText", genre },
                { "limit", 1 },
                { "additionalData", genre },
                { "resultsEndpoint", resultsEndpoint }
            };

            string jsonPayload = JsonConvert.SerializeObject(registerPayload, Formatting.None);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Register with Home Screen Sections plugin
            _logger.LogInformation("[Genre Manager] Registering section with ID: {SectionId}, endpoint: {Endpoint}, payload: {Payload}", sectionId, resultsEndpoint, jsonPayload);
            HttpResponseMessage response = client.PostAsync("/HomeScreen/RegisterSection", content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                _logger.LogError("[Genre Manager] Registration failed for {Genre}: {Status} - {Error}", genre, response.StatusCode, errorContent);
                throw new Exception($"Registration failed with status {response.StatusCode}: {errorContent}");
            }
            else
            {
                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                _logger.LogInformation("[Genre Manager] Registration response for {Genre}: {Response}", genre, responseContent);
            }
        }
    }
}
