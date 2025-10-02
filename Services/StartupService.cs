using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.GenreManager.Services
{
    /// <summary>
    /// Startup service that registers File Transformation on Jellyfin startup.
    /// </summary>
    public class StartupService : IScheduledTask
    {
        private readonly ILogger<StartupService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StartupService(ILogger<StartupService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public string Name => "Genre Manager Startup";

        /// <inheritdoc />
        public string Key => "GenreManagerStartup";

        /// <inheritdoc />
        public string Description => "Registers Genre Manager transformations on startup";

        /// <inheritdoc />
        public string Category => "Genre Manager";

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Genre Manager: Starting up, registering file transformations");

            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                _logger.LogError("Genre Manager: Configuration not found");
                return Task.CompletedTask;
            }

            if (config.UseFileTransformation)
            {
                _ = RegisterTransformation();
            }
            else
            {
                _logger.LogInformation("Genre Manager: File Transformation disabled in settings");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo
            {
                Type = TaskTriggerInfo.TriggerStartup
            };
        }

        private Task RegisterTransformation()
        {
            try
            {
                _logger.LogInformation("Genre Manager: Starting RegisterTransformation");

                // Try a more permissive regex pattern that matches index.html anywhere in the path
                JObject data = new JObject
                {
                    { "id", Plugin.Instance!.Id.ToString() },
                    { "fileNamePattern", ".*index\\.html$" },
                    { "callbackAssembly", GetType().Assembly.FullName },
                    { "callbackClass", typeof(Helpers.Transformations).FullName },
                    { "callbackMethod", nameof(Helpers.Transformations.IndexTransformation) }
                };

                _logger.LogInformation("Genre Manager: Registration data prepared - Assembly: {Assembly}, Class: {Class}, Method: {Method}",
                    GetType().Assembly.FullName,
                    typeof(Helpers.Transformations).FullName,
                    nameof(Helpers.Transformations.IndexTransformation));

                Assembly? fileTransformationAssembly = AssemblyLoadContext.All
                    .SelectMany(x => x.Assemblies)
                    .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") ?? false);

                if (fileTransformationAssembly != null)
                {
                    _logger.LogInformation("Genre Manager: Found File Transformation assembly: {Assembly}", fileTransformationAssembly.FullName);

                    Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");
                    if (pluginInterfaceType != null)
                    {
                        _logger.LogInformation("Genre Manager: Found PluginInterface type, invoking RegisterTransformation");

                        var method = pluginInterfaceType.GetMethod("RegisterTransformation");
                        if (method != null)
                        {
                            method.Invoke(null, new object?[] { data });
                            _logger.LogInformation("Genre Manager: File Transformation registered successfully");
                        }
                        else
                        {
                            _logger.LogWarning("Genre Manager: RegisterTransformation method not found in PluginInterface");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Genre Manager: File Transformation PluginInterface not found in assembly");
                    }
                }
                else
                {
                    _logger.LogWarning("Genre Manager: File Transformation plugin not found. Please install it from the plugin catalog.");
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Genre Manager: Error registering File Transformation");
                return Task.CompletedTask;
            }
        }
    }
}
