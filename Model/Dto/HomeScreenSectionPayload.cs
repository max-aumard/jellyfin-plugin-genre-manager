using System;

namespace Jellyfin.Plugin.GenreManager.Model.Dto
{
    /// <summary>
    /// Payload for home screen section requests.
    /// </summary>
    public class HomeScreenSectionPayload
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets additional data for the section.
        /// </summary>
        public string? AdditionalData { get; set; }
    }
}
