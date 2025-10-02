using System;
using Jellyfin.Plugin.GenreManager.HomeScreen.Sections;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreSectionController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="dtoService">The DTO service.</param>
        public GenreSectionController(
            IUserManager userManager,
            ILibraryManager libraryManager,
            IDtoService dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
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
                // Extract userId and additionalData from payload
                Guid userId = payload["UserId"]?.ToObject<Guid>() ?? Guid.Empty;
                string? additionalData = payload["AdditionalData"]?.ToObject<string>();

                if (userId == Guid.Empty)
                {
                    return BadRequest("UserId is required");
                }

                // Use AdditionalData if provided (it contains the genre name), otherwise use route parameter
                string genreName = !string.IsNullOrEmpty(additionalData) ? additionalData : genre;

                var config = Plugin.Instance?.Configuration;
                if (config == null)
                {
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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting genre results: {ex.Message}");
            }
        }
    }
}
