using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using Lists2Playlists.Configuration;
using Lists2Playlists.Providers;

namespace Lists2Playlists.Services
{
    /// <summary>
    /// Service for managing plugin configuration
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private const string ConfigFileName = "Lists2Playlists.json";
        private readonly IApplicationPaths _applicationPaths;
        private readonly IJsonSerializer _jsonSerializer;
        private PluginConfiguration? _cachedConfiguration;

        public ConfigurationService(IApplicationPaths applicationPaths, IJsonSerializer jsonSerializer)
        {
            _applicationPaths = applicationPaths ?? throw new ArgumentNullException(nameof(applicationPaths));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public PluginConfiguration GetConfiguration()
        {
            if (_cachedConfiguration != null)
                return _cachedConfiguration;

            var configPath = GetConfigurationPath();
            if (File.Exists(configPath))
            {
                try
                {
                    var json = File.ReadAllText(configPath);
                    _cachedConfiguration = _jsonSerializer.DeserializeFromString<PluginConfiguration>(json);
                    return _cachedConfiguration ?? new PluginConfiguration();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading configuration: {ex.Message}");
                }
            }

            _cachedConfiguration = new PluginConfiguration();
            return _cachedConfiguration;
        }

        public void SaveConfiguration(PluginConfiguration configuration)
        {
            _cachedConfiguration = configuration;
            var configPath = GetConfigurationPath();
            var configDir = Path.GetDirectoryName(configPath);
            
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir!);

            try
            {
                var json = _jsonSerializer.SerializeToString(configuration);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                throw;
            }
        }

        public ListConfiguration? GetListConfiguration(string listId)
        {
            var config = GetConfiguration();
            return config.ListConfigurations?.FirstOrDefault(lc => lc.Id == listId);
        }

        public void SaveListConfiguration(ListConfiguration configuration)
        {
            var config = GetConfiguration();
            
            var existingIndex = config.ListConfigurations.FindIndex(lc => lc.Id == configuration.Id);
            if (existingIndex >= 0)
            {
                config.ListConfigurations[existingIndex] = configuration;
            }
            else
            {
                config.ListConfigurations.Add(configuration);
            }

            SaveConfiguration(config);
        }

        public void RemoveListConfiguration(string listId)
        {
            var config = GetConfiguration();
            config.ListConfigurations.RemoveAll(lc => lc.Id == listId);
            SaveConfiguration(config);
        }

        private string GetConfigurationPath()
        {
            var pluginConfigDir = Path.Combine(_applicationPaths.PluginConfigurationsPath, "Lists2Playlists");
            return Path.Combine(pluginConfigDir, ConfigFileName);
        }
    }
}
