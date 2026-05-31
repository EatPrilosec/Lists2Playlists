using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Lists2Playlists.Configuration;

namespace Lists2Playlists
{
    /// <summary>
    /// Lists2Playlists plugin
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private const string PluginGuid = "b9cf77fa-aed8-4e3f-a7f3-4a1c5c8a9b7d";
        private const string PluginName = "Lists2Playlists";
        private const string PluginVersion = "1.0.0";

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin? Instance { get; private set; }

        /// <summary>
        /// Gets the name of the plugin
        /// </summary>
        public override string Name => PluginName;

        /// <summary>
        /// Gets the description of the plugin
        /// </summary>
        public override string Description => "Create Emby playlists from online list services like Trakt, SIMKL, Letterboxd, and more";

        /// <summary>
        /// Gets the unique id of the plugin
        /// </summary>
        public override Guid Id => new Guid(PluginGuid);

        /// <summary>
        /// Gets the web pages
        /// </summary>
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new List<PluginPageInfo>
            {
                new PluginPageInfo
                {
                    Name = "Lists2Playlists Settings",
                    EmbeddedResourcePath = "Lists2Playlists.Configuration.configPage.html",
                    DisplayName = "Lists2Playlists"
                }
            };
        }
    }
}
