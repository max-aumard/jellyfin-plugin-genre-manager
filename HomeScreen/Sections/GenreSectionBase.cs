using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using Jellyfin.Plugin.GenreManager.Library;
using Jellyfin.Plugin.GenreManager.Model.Dto;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// Base class for genre-based home screen sections.
    /// </summary>
    public abstract class GenreSectionBase : IHomeScreenSection
    {
        /// <summary>
        /// Gets the genre name for this section.
        /// </summary>
        protected abstract string GenreName { get; }

        /// <inheritdoc />
        public string? Section => $"Genre_{GenreName.Replace(" ", "")}";

        /// <inheritdoc />
        public string? DisplayText { get; set; }

        /// <inheritdoc />
        public int? Limit => 1;

        /// <inheritdoc />
        public string? Route => "genres";

        /// <inheritdoc />
        public string? AdditionalData { get; set; }

        /// <inheritdoc />
        public object? OriginalPayload { get; set; }

        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreSectionBase"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="dtoService">The DTO service.</param>
        protected GenreSectionBase(
            IUserManager userManager,
            ILibraryManager libraryManager,
            IDtoService dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
            DisplayText = GenreName;
            AdditionalData = GenreName;
        }

        /// <inheritdoc />
        public QueryResult<BaseItemDto> GetResults(HomeScreenSectionPayload payload, IQueryCollection queryCollection)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                return new QueryResult<BaseItemDto>(Array.Empty<BaseItemDto>());
            }

            // Only return results if this genre is enabled in configuration
            if (!config.SelectedGenres.Contains(GenreName))
            {
                return new QueryResult<BaseItemDto>(Array.Empty<BaseItemDto>());
            }

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

            User? user = _userManager.GetUserById(payload.UserId);

            var query = new InternalItemsQuery(user)
            {
                Genres = new[] { GenreName },
                Limit = config.ItemsPerSection,
                OrderBy = new[]
                {
                    (ItemSortBy.Random, SortOrder.Ascending)
                },
                Recursive = true
            };

            // Filter by type
            if (config.ShowOnlyMovies)
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

        /// <inheritdoc />
        public abstract IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null);

        /// <inheritdoc />
        public HomeScreenSectionInfo GetInfo()
        {
            return new HomeScreenSectionInfo
            {
                Section = Section,
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                Route = Route,
                Limit = Limit ?? 1,
                OriginalPayload = OriginalPayload,
                ViewMode = SectionViewMode.Landscape
            };
        }
    }
}
