using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.GenreManager.Controllers
{
    /// <summary>
    /// Genre Manager API controller.
    /// </summary>
    [ApiController]
    [Route("GenreManager")]
    public class GenreManagerController : ControllerBase
    {
        private readonly string _scriptPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreManagerController"/> class.
        /// </summary>
        public GenreManagerController()
        {
            _scriptPath = GetType().Namespace?.Replace(".Controllers", "") + ".ClientScripts.genreDisplay.js";
        }

        /// <summary>
        /// Gets the client script.
        /// </summary>
        /// <returns>The JavaScript file.</returns>
        [HttpGet("script")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/javascript")]
        public ActionResult GetClientScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var scriptStream = assembly.GetManifestResourceStream(_scriptPath);

            if (scriptStream != null)
            {
                return File(scriptStream, "application/javascript");
            }

            return NotFound("Script not found");
        }

        /// <summary>
        /// Gets the plugin configuration for client use.
        /// </summary>
        /// <returns>The configuration object.</returns>
        [HttpGet("config")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public ActionResult GetConfig()
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                return NotFound("Configuration not found");
            }

            return Ok(new
            {
                selectedGenres = config.SelectedGenres,
                itemsPerSection = config.ItemsPerSection,
                showOnlyMovies = config.ShowOnlyMovies
            });
        }
    }
}
