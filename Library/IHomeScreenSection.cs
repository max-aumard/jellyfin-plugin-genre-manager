using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Model.Dto;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.GenreManager.Library
{
    /// <summary>
    /// Section view mode.
    /// </summary>
    public enum SectionViewMode
    {
        /// <summary>
        /// Landscape view.
        /// </summary>
        Landscape,

        /// <summary>
        /// Portrait view.
        /// </summary>
        Portrait
    }

    /// <summary>
    /// Interface for home screen sections.
    /// </summary>
    public interface IHomeScreenSection
    {
        /// <summary>
        /// Gets the section identifier.
        /// </summary>
        public string? Section { get; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        public string? DisplayText { get; set; }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        public int? Limit { get; }

        /// <summary>
        /// Gets the route.
        /// </summary>
        public string? Route { get; }

        /// <summary>
        /// Gets or sets additional data.
        /// </summary>
        public string? AdditionalData { get; set; }

        /// <summary>
        /// Gets the original payload.
        /// </summary>
        public object? OriginalPayload { get; }

        /// <summary>
        /// Gets the results for this section.
        /// </summary>
        /// <param name="payload">The section payload.</param>
        /// <param name="queryCollection">The query collection.</param>
        /// <returns>Query result with items.</returns>
        public QueryResult<BaseItemDto> GetResults(HomeScreenSectionPayload payload, IQueryCollection queryCollection);

        /// <summary>
        /// Creates a new instance of this section.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="otherInstances">Other section instances.</param>
        /// <returns>New section instance.</returns>
        public IHomeScreenSection CreateInstance(Guid? userId, IEnumerable<IHomeScreenSection>? otherInstances = null);

        /// <summary>
        /// Gets section info.
        /// </summary>
        /// <returns>Section info.</returns>
        public HomeScreenSectionInfo GetInfo();
    }

    /// <summary>
    /// Information about a home screen section.
    /// </summary>
    public class HomeScreenSectionInfo
    {
        /// <summary>
        /// Gets or sets the section identifier.
        /// </summary>
        public string? Section { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        public string? DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int Limit { get; set; } = 1;

        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        public string? Route { get; set; }

        /// <summary>
        /// Gets or sets additional data.
        /// </summary>
        public string? AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the container class.
        /// </summary>
        public string? ContainerClass { get; set; }

        /// <summary>
        /// Gets or sets the view mode.
        /// </summary>
        public SectionViewMode? ViewMode { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to display title text.
        /// </summary>
        public bool DisplayTitleText { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show the details menu.
        /// </summary>
        public bool ShowDetailsMenu { get; set; } = true;

        /// <summary>
        /// Gets or sets the original payload.
        /// </summary>
        public object? OriginalPayload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether view mode can be changed.
        /// </summary>
        public bool AllowViewModeChange { get; set; } = true;
    }
}
