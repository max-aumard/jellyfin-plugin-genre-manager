using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.GenreManager.Helpers
{
    /// <summary>
    /// HTML transformation methods for File Transformation plugin.
    /// </summary>
    public static class Transformations
    {
        /// <summary>
        /// Transforms index.html to inject Genre Manager script.
        /// </summary>
        /// <param name="payload">The transformation payload containing HTML contents.</param>
        /// <returns>Modified HTML content.</returns>
        public static string IndexTransformation(Models.PatchRequestPayload payload)
        {
            if (Plugin.Instance == null)
            {
                return payload.Contents ?? string.Empty;
            }

            // Inject script tag before </body>
            // Note: Base path is handled by Jellyfin automatically
            string script = "<script FileTransformation=\"true\" plugin=\"GenreManager\" defer=\"defer\" src=\"/GenreManager/script\"></script>";

            string text = Regex.Replace(payload.Contents ?? string.Empty, "(</body>)", $"{script}$1");

            return text;
        }
    }
}
