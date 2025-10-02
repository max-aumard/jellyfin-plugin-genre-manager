using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// Drama genre section.
    /// </summary>
    public class DramaGenreSection : GenreSectionBase
    {
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <inheritdoc />
        protected override string GenreName => "Drama";

        /// <summary>
        /// Initializes a new instance of the <see cref="DramaGenreSection"/> class.
        /// </summary>
        public DramaGenreSection(IUserManager userManager, ILibraryManager libraryManager, IDtoService dtoService)
            : base(userManager, libraryManager, dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public override IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
        {
            return new DramaGenreSection(_userManager, _libraryManager, _dtoService)
            {
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                OriginalPayload = null
            };
        }
    }
}
