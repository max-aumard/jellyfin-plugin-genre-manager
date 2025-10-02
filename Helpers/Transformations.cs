using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

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
        /// <param name="payload">The transformation payload as JObject containing HTML contents.</param>
        /// <returns>Modified HTML content.</returns>
        public static string IndexTransformation(JObject payload)
        {
            string debugPath = Path.Combine(Path.GetTempPath(), "genremanager-transform.log");

            try
            {
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Transformation called with JObject\n");

                // Extract contents from JObject
                string? contents = payload["contents"]?.ToString();

                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Contents length: {contents?.Length ?? 0}\n");

                if (Plugin.Instance == null)
                {
                    File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Plugin instance is null\n");
                    return contents ?? string.Empty;
                }

                if (string.IsNullOrEmpty(contents))
                {
                    File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Contents is null or empty\n");
                    return contents ?? string.Empty;
                }

                // Inject script tag before </body>
                string script = "<script FileTransformation=\"true\" plugin=\"GenreManager\" defer=\"defer\" src=\"/GenreManager/script\"></script>";

                string text = Regex.Replace(contents, "(</body>)", $"{script}$1", RegexOptions.IgnoreCase);

                bool wasInjected = text != contents;
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Script injection {(wasInjected ? "SUCCESS" : "FAILED - </body> tag not found")}\n");

                return text;
            }
            catch (Exception ex)
            {
                File.AppendAllText(debugPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error: {ex.Message}\n{ex.StackTrace}\n");
                return string.Empty;
            }
        }
    }
}
