using System;
using System.IO;
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
            try
            {
                // Write debug log to a file to verify the transformation is being called
                string debugPath = Path.Combine(Path.GetTempPath(), "genremanager-transform.log");
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Transformation called - Contents length: {payload.Contents?.Length ?? 0}\n");

                if (Plugin.Instance == null)
                {
                    File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Plugin instance is null\n");
                    return payload.Contents ?? string.Empty;
                }

                // Inject script tag before </body>
                string script = "<script FileTransformation=\"true\" plugin=\"GenreManager\" defer=\"defer\" src=\"/GenreManager/script\"></script>";

                string text = Regex.Replace(payload.Contents ?? string.Empty, "(</body>)", $"{script}$1", RegexOptions.IgnoreCase);

                bool wasInjected = text != payload.Contents;
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Script injection {(wasInjected ? "SUCCESS" : "FAILED - </body> tag not found")}\n");

                return text;
            }
            catch (Exception ex)
            {
                string debugPath = Path.Combine(Path.GetTempPath(), "genremanager-transform.log");
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error: {ex.Message}\n{ex.StackTrace}\n");
                return payload.Contents ?? string.Empty;
            }
        }
    }
}
