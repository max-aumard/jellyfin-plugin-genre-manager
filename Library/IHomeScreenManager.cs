using System;
using System.Collections.Generic;
using Jellyfin.Plugin.GenreManager.Model.Dto;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.GenreManager.Library
{
    /// <summary>
    /// Interface for home screen manager.
    /// </summary>
    public interface IHomeScreenManager
    {
        /// <summary>
        /// Registers a results delegate.
        /// </summary>
        /// <typeparam name="T">The section type.</typeparam>
        void RegisterResultsDelegate<T>()
            where T : IHomeScreenSection;

        /// <summary>
        /// Registers a results delegate.
        /// </summary>
        /// <typeparam name="T">The section type.</typeparam>
        /// <param name="handler">The handler.</param>
        void RegisterResultsDelegate<T>(T handler)
            where T : IHomeScreenSection;

        /// <summary>
        /// Gets section types.
        /// </summary>
        /// <returns>Section types.</returns>
        IEnumerable<IHomeScreenSection> GetSectionTypes();

        /// <summary>
        /// Invokes a results delegate.
        /// </summary>
        /// <param name="key">The section key.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="queryCollection">The query collection.</param>
        /// <returns>Query result.</returns>
        QueryResult<BaseItemDto> InvokeResultsDelegate(string key, HomeScreenSectionPayload payload, IQueryCollection queryCollection);
    }
}
