using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.GenreManager.Models
{
    /// <summary>
    /// Payload received from File Transformation plugin.
    /// </summary>
    public class PatchRequestPayload
    {
        /// <summary>
        /// Gets or sets the file contents.
        /// </summary>
        [JsonPropertyName("contents")]
        public string? Contents { get; set; }
    }
}
