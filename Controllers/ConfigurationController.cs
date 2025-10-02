using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.GenreManager.Controllers
{
    /// <summary>
    /// Configuration controller.
    /// </summary>
    [ApiController]
    [Route("GenreManager")]
    public class ConfigurationController : ControllerBase
    {
        /// <summary>
        /// Gets the configuration page.
        /// </summary>
        /// <returns>The configuration page HTML.</returns>
        [HttpGet("ConfigurationPage")]
        [AllowAnonymous]
        public ActionResult GetConfigurationPage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Jellyfin.Plugin.GenreManager.Configuration.configPage.html";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return NotFound("Configuration page not found");
            }

            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            return Content(content, "text/html");
        }

        /// <summary>
        /// Gets the client script.
        /// </summary>
        /// <returns>The JavaScript file.</returns>
        [HttpGet("genreDisplay.js")]
        [AllowAnonymous]
        public ActionResult GetClientScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Jellyfin.Plugin.GenreManager.ClientScripts.genreDisplay.js";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return NotFound("Script not found");
            }

            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            return Content(content, "application/javascript");
        }
    }
}
