using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.GenreManager
{
    public class Plugin : BasePlugin<PluginConfiguration>
    {
        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public override string Name => "Genre Manager";

        public override Guid Id => Guid.Parse("014df46b-ffdc-414f-b841-e7fb5546b8e5");

        public static Plugin? Instance { get; private set; }

        public override string Description =>
            "Affiche automatiquement les genres de films en rangées horizontales style Netflix sur la page d'accueil";
    }
}
