using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// Animation genre section.
    /// </summary>
    public class AnimationGenreSection : GenreSectionBase
    {
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <inheritdoc />
        protected override string GenreName => "Animation";

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationGenreSection"/> class.
        /// </summary>
        public AnimationGenreSection(IUserManager userManager, ILibraryManager libraryManager, IDtoService dtoService)
            : base(userManager, libraryManager, dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public override IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
        {
            return new AnimationGenreSection(_userManager, _libraryManager, _dtoService)
            {
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                OriginalPayload = null
            };
        }
    }
}
