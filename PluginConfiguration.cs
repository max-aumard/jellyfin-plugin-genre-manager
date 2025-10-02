using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.GenreManager
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            // Genres par défaut sélectionnés (noms anglais = noms Jellyfin natifs)
            SelectedGenres = new List<string>
            {
                "Action", "Comedy", "Drama", "Science Fiction",
                "Horror", "Romance", "Thriller", "Animation"
            };

            ItemsPerSection = 20;
            ShowOnlyMovies = true;
            UseFileTransformation = true;
        }

        /// <summary>
        /// Gets or sets the selected genres to display on home page.
        /// </summary>
        public List<string> SelectedGenres { get; set; }

        /// <summary>
        /// Gets or sets the number of items to display per genre section.
        /// </summary>
        public int ItemsPerSection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show only movies (exclude TV series).
        /// </summary>
        public bool ShowOnlyMovies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use File Transformation plugin for script injection.
        /// </summary>
        public bool UseFileTransformation { get; set; }
    }
}
