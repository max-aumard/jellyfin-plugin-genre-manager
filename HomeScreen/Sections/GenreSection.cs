using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// Home screen section for a specific genre.
    /// </summary>
    public class GenreSection
    {
        /// <summary>
        /// Gets the genre name.
        /// </summary>
        public string GenreName { get; }

        /// <summary>
        /// Gets the section ID.
        /// </summary>
        public string SectionId => $"Genre_{GenreName.Replace(" ", "")}";

        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreSection"/> class.
        /// </summary>
        /// <param name="genreName">The genre name.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="dtoService">The DTO service.</param>
        public GenreSection(
            string genreName,
            IUserManager userManager,
            ILibraryManager libraryManager,
            IDtoService dtoService)
        {
            GenreName = genreName;
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <summary>
        /// Gets the results for this genre section.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="itemsPerSection">Number of items to return.</param>
        /// <param name="showOnlyMovies">Whether to show only movies or include TV series.</param>
        /// <returns>Query result with items for this genre.</returns>
        public QueryResult<BaseItemDto> GetResults(Guid userId, int itemsPerSection, bool showOnlyMovies)
        {
            var dtoOptions = new DtoOptions
            {
                Fields = new List<ItemFields>
                {
                    ItemFields.PrimaryImageAspectRatio,
                    ItemFields.Path
                }
            };

            dtoOptions.ImageTypeLimit = 1;
            dtoOptions.ImageTypes = new List<ImageType>
            {
                ImageType.Primary,
                ImageType.Backdrop,
                ImageType.Thumb
            };

            User? user = _userManager.GetUserById(userId);

            var query = new InternalItemsQuery(user)
            {
                Genres = new[] { GenreName },
                Limit = itemsPerSection,
                OrderBy = new[]
                {
                    (ItemSortBy.Random, SortOrder.Ascending)
                },
                Recursive = true
            };

            // Filter by type
            if (showOnlyMovies)
            {
                query.IncludeItemTypes = new[] { BaseItemKind.Movie };
            }
            else
            {
                query.IncludeItemTypes = new[] { BaseItemKind.Movie, BaseItemKind.Series };
            }

            IReadOnlyList<BaseItem> items = _libraryManager.GetItemList(query);

            return new QueryResult<BaseItemDto>(
                Array.ConvertAll(items.ToArray(), i => _dtoService.GetBaseItemDto(i, dtoOptions, user)));
        }
    }
}
