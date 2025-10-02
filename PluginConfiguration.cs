using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.GenreManager
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            // Genres par défaut (les 10 basiques)
            SelectedGenres = new List<string>
            {
                "Action", "Comédie", "Drame", "Science-Fiction",
                "Horreur", "Romance", "Thriller", "Animation",
                "Aventure", "Documentaire"
            };

            GenreOrdering = new List<GenreOrder>
            {
                new() { GenreName = "Action", Priority = 1 },
                new() { GenreName = "Comédie", Priority = 2 },
                new() { GenreName = "Science-Fiction", Priority = 3 },
                new() { GenreName = "Drame", Priority = 4 },
                new() { GenreName = "Horreur", Priority = 5 },
                new() { GenreName = "Romance", Priority = 6 },
                new() { GenreName = "Thriller", Priority = 7 },
                new() { GenreName = "Animation", Priority = 8 },
                new() { GenreName = "Aventure", Priority = 9 },
                new() { GenreName = "Documentaire", Priority = 10 }
            };

            ItemsPerSection = 20;
            ShowOnlyMovies = true;
        }

        // Genres sélectionnés pour affichage
        public List<string> SelectedGenres { get; set; }

        // Ordre de priorité des genres
        public List<GenreOrder> GenreOrdering { get; set; }

        // Nombre d'éléments par section
        public int ItemsPerSection { get; set; }

        // Afficher uniquement les films (sinon films + séries)
        public bool ShowOnlyMovies { get; set; }
    }

    public class GenreOrder
    {
        public string GenreName { get; set; } = string.Empty;
        public int Priority { get; set; }
    }
}
