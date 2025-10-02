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
    /// Home screen section for a specific genre.
    /// </summary>
    public class GenreSection : IHomeScreenSection
    {
        /// <summary>
        /// Gets the genre name.
        /// </summary>
        public string GenreName { get; }

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
            DisplayText = genreName;
            AdditionalData = genreName;
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public QueryResult<BaseItemDto> GetResults(HomeScreenSectionPayload payload, IQueryCollection queryCollection)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null)
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
        public IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
        {
            return new GenreSection(
                GenreName,
                _userManager,
                _libraryManager,
                _dtoService)
            {
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                OriginalPayload = null
            };
        }

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
