using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.GenreManager.HomeScreen.Sections;
using Jellyfin.Plugin.GenreManager.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.GenreManager.Services
{
    /// <summary>
    /// Startup service that registers genre sections with Home Screen Sections plugin.
    /// </summary>
    public class GenreRegistrationService : IScheduledTask
    {
        private readonly ILogger<GenreRegistrationService> _logger;
        private readonly IHomeScreenManager _homeScreenManager;
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreRegistrationService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="homeScreenManager">The home screen manager.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="dtoService">The DTO service.</param>
        public GenreRegistrationService(
            ILogger<GenreRegistrationService> logger,
            IHomeScreenManager homeScreenManager,
            IUserManager userManager,
            ILibraryManager libraryManager,
            IDtoService dtoService)
        {
            _logger = logger;
            _homeScreenManager = homeScreenManager;
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public string Name => "Genre Manager Registration";

        /// <inheritdoc />
        public string Key => "GenreManagerRegistration";

        /// <inheritdoc />
        public string Description => "Registers genre sections with Home Screen Sections plugin";

        /// <inheritdoc />
        public string Category => "Genre Manager";

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Genre Manager] Registering genre sections with HomeScreen");

            var config = Plugin.Instance?.Configuration;
            if (config == null || config.SelectedGenres == null || !config.SelectedGenres.Any())
            {
                _logger.LogWarning("[Genre Manager] No genres configured");
                return Task.CompletedTask;
            }

            try
            {
                // Remove duplicates from selected genres
                var uniqueGenres = config.SelectedGenres.Distinct().ToList();
                _logger.LogInformation("[Genre Manager] Registering {Count} unique genres", uniqueGenres.Count);

                int registeredCount = 0;

                foreach (var genre in uniqueGenres)
                {
                    try
                    {
                        var genreSection = new GenreSection(
                            genre,
                            _userManager,
                            _libraryManager,
                            _dtoService);

                        _homeScreenManager.RegisterResultsDelegate(genreSection);
                        registeredCount++;

                        _logger.LogInformation("[Genre Manager] Registered section for genre: {Genre} with ID: {SectionId}",
                            genre, genreSection.Section);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[Genre Manager] Error registering genre section: {Genre}", genre);
                    }
                }

                _logger.LogInformation("[Genre Manager] Successfully registered {Count} genre sections", registeredCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Genre Manager] Error registering genre sections");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public System.Collections.Generic.IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo
            {
                Type = TaskTriggerInfo.TriggerStartup
            };
        }
    }
}
