using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// Music genre section.
    /// </summary>
    public class MusicGenreSection : GenreSectionBase
    {
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <inheritdoc />
        protected override string GenreName => "Music";

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicGenreSection"/> class.
        /// </summary>
        public MusicGenreSection(IUserManager userManager, ILibraryManager libraryManager, IDtoService dtoService)
            : base(userManager, libraryManager, dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public override IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
        {
            return new MusicGenreSection(_userManager, _libraryManager, _dtoService)
            {
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                OriginalPayload = null
            };
        }
    }
}
