$genres = @(
    "Action",
    "Adventure",
    "Animation",
    "Biography",
    "Comedy",
    "Crime",
    "Documentary",
    "Drama",
    "Family",
    "Fantasy",
    "History",
    "Horror",
    "Music",
    "Mystery",
    "Romance",
    "Science Fiction",
    "Sport",
    "Thriller",
    "War",
    "Western"
)

$template = @'
using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Library;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.GenreManager.HomeScreen.Sections
{
    /// <summary>
    /// {0} genre section.
    /// </summary>
    public class {1}GenreSection : GenreSectionBase
    {
        private readonly IUserManager _userManager;
        private readonly ILibraryManager _libraryManager;
        private readonly IDtoService _dtoService;

        /// <inheritdoc />
        protected override string GenreName => "{0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="{1}GenreSection"/> class.
        /// </summary>
        public {1}GenreSection(IUserManager userManager, ILibraryManager libraryManager, IDtoService dtoService)
            : base(userManager, libraryManager, dtoService)
        {
            _userManager = userManager;
            _libraryManager = libraryManager;
            _dtoService = dtoService;
        }

        /// <inheritdoc />
        public override IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null)
        {
            return new {1}GenreSection(_userManager, _libraryManager, _dtoService)
            {
                DisplayText = DisplayText,
                AdditionalData = AdditionalData,
                OriginalPayload = null
            };
        }
    }
}
'@

foreach ($genre in $genres) {
    $className = $genre -replace ' ', ''
    $fileName = "$className" + "GenreSection.cs"
    $filePath = Join-Path "HomeScreen\Sections" $fileName

    $content = $template -f $genre, $className
    Set-Content -Path $filePath -Value $content -Encoding UTF8
    Write-Host "Created $filePath"
}

Write-Host "`nGenerated $($genres.Count) genre section classes"
