using System;
using Jellyfin.Plugin.GenreManager.HomeScreen.Sections;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.GenreManager.Controllers
{
    /// <summary>
    /// Genre section endpoints for HomeScreen integration.
    /// </summary>
    [ApiController]
    [Route("GenreSection")]
    public class GenreSectionController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;
        private readonly ILogger<GenreSectionController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreSectionController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="dtoService">The DTO service.</param>
        /// <param name="logger">The logger.</param>
        public GenreSectionController(
            IUserManager userManager,
            ILibraryManager libraryManager,
            IDtoService dtoService,
            ILogger<GenreSectionController> logger)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
            _logger = logger;
        }

        /// <summary>
        /// Gets results for a specific genre.
        /// </summary>
        /// <param name="genre">The genre name.</param>
        /// <param name="payload">The section payload from HomeScreen plugin.</param>
        /// <returns>Query result with items for the genre.</returns>
        [HttpPost("{genre}")]
        [Authorize]
        public ActionResult<QueryResult<BaseItemDto>> GetGenreResults(
            [FromRoute] string genre,
            [FromBody] JObject payload)
        {
            try
            {
                _logger.LogInformation("[Genre Manager] GetGenreResults called for genre: {Genre}, payload: {Payload}", genre, payload.ToString());

                // Extract userId and additionalData from payload
                Guid userId = payload["UserId"]?.ToObject<Guid>() ?? Guid.Empty;
                string? additionalData = payload["AdditionalData"]?.ToObject<string>();

                _logger.LogInformation("[Genre Manager] UserId: {UserId}, AdditionalData: {AdditionalData}", userId, additionalData);

                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("[Genre Manager] UserId is empty or invalid");
                    return BadRequest("UserId is required");
                }

                // Use AdditionalData if provided (it contains the genre name), otherwise use route parameter
                string genreName = !string.IsNullOrEmpty(additionalData) ? additionalData : genre;

                _logger.LogInformation("[Genre Manager] Using genre name: {GenreName}", genreName);

                var config = Plugin.Instance?.Configuration;
                if (config == null)
                {
                    _logger.LogError("[Genre Manager] Plugin configuration not found");
                    return BadRequest("Plugin configuration not found");
                }

                // Create genre section
                var genreSection = new GenreSection(
                    genreName,
                    _userManager,
                    _libraryManager,
                    _dtoService);

                // Get results
                var result = genreSection.GetResults(
                    userId,
                    config.ItemsPerSection,
                    config.ShowOnlyMovies);

                _logger.LogInformation("[Genre Manager] Returning {Count} items for genre {Genre}", result.TotalRecordCount, genreName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Genre Manager] Error getting genre results: {Message}", ex.Message);
                return StatusCode(500, $"Error getting genre results: {ex.Message}");
            }
        }
    }
}
