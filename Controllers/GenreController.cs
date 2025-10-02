using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.GenreManager.Controllers
{
    [ApiController]
    [Route("api/genremanager")]
    [Authorize]
    public class GenreController : ControllerBase
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IUserManager _userManager;

        public GenreController(
            ILibraryManager libraryManager,
            IUserManager userManager)
        {
            _libraryManager = libraryManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Récupère tous les genres disponibles dans la bibliothèque
        /// </summary>
        [HttpGet("genres")]
        public ActionResult<List<string>> GetGenres([FromQuery] Guid userId)
        {
            var user = _userManager.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Utilisateur introuvable");
            }

            var query = new InternalItemsQuery(user)
            {
                IncludeItemTypes = new[] { BaseItemKind.Movie, BaseItemKind.Series },
                Recursive = true
            };

            var items = _libraryManager.GetItemsResult(query);

            var genres = items.Items
                .SelectMany(i => i.Genres)
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            return Ok(genres);
        }

        /// <summary>
        /// Récupère les films/séries d'un genre spécifique
        /// </summary>
        [HttpGet("section/{genreName}")]
        public ActionResult<object> GetGenreSection(
            [FromRoute] string genreName,
            [FromQuery] Guid userId,
            [FromQuery] int startIndex = 0,
            [FromQuery] int limit = 20)
        {
            var user = _userManager.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Utilisateur introuvable");
            }

            // Validation du nom de genre
            if (string.IsNullOrWhiteSpace(genreName) || genreName.Length > 100)
            {
                return BadRequest("Nom de genre invalide");
            }

            var config = Plugin.Instance?.Configuration;
            var itemTypes = new List<BaseItemKind> { BaseItemKind.Movie };

            if (config != null && !config.ShowOnlyMovies)
            {
                itemTypes.Add(BaseItemKind.Series);
            }

            var query = new InternalItemsQuery(user)
            {
                IncludeItemTypes = itemTypes.ToArray(),
                Genres = new[] { genreName },
                Recursive = true,
                StartIndex = startIndex,
                Limit = limit,
                OrderBy = new[]
                {
                    (ItemSortBy.DateCreated, SortOrder.Descending)
                }
            };

            var items = _libraryManager.GetItemsResult(query);

            var result = new
            {
                Items = items.Items.Select(i => new
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = i.GetType().Name,
                    Genres = i.Genres,
                    Overview = i.Overview,
                    PremiereDate = i.PremiereDate,
                    ImageTags = i.ImageInfos?.ToDictionary(img => img.Type.ToString(), img => img.Path)
                }).ToArray(),
                TotalRecordCount = items.TotalRecordCount,
                StartIndex = startIndex
            };

            return Ok(result);
        }

        /// <summary>
        /// Obtient la configuration des sections de genres pour un utilisateur
        /// </summary>
        [HttpGet("configuration")]
        public ActionResult<object> GetGenreSections([FromQuery] Guid userId)
        {
            var user = _userManager.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Utilisateur introuvable");
            }

            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                return NotFound("Configuration introuvable");
            }

            // Retourner les genres triés par priorité
            var orderedGenres = config.GenreOrdering
                .Where(g => config.SelectedGenres.Contains(g.GenreName))
                .OrderBy(g => g.Priority)
                .Select(g => new
                {
                    genreName = g.GenreName,
                    priority = g.Priority,
                    apiEndpoint = $"/api/genremanager/section/{Uri.EscapeDataString(g.GenreName)}?userId={userId}&limit={config.ItemsPerSection}"
                })
                .ToList();

            return Ok(new
            {
                genres = orderedGenres,
                itemsPerSection = config.ItemsPerSection
            });
        }
    }
}
