using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Tasks;
using Lists2Playlists.Configuration;
using Lists2Playlists.Providers;
using Lists2Playlists.Services;
using Lists2Playlists.Sync;

namespace Lists2Playlists.ScheduledTasks
{
    /// <summary>
    /// Scheduled task for syncing lists
    /// </summary>
    public class SyncListsTask : IScheduledTask
    {
        private readonly ISyncOrchestrator _syncOrchestrator;
        private readonly IConfigurationService _configurationService;

        public SyncListsTask(ISyncOrchestrator syncOrchestrator, IConfigurationService configurationService)
        {
            _syncOrchestrator = syncOrchestrator ?? throw new ArgumentNullException(nameof(syncOrchestrator));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        public string Name => "Sync Lists2Playlists";

        public string Description => "Syncs configured lists from Trakt, SIMKL, and other services with Emby playlists";

        public string Category => "Plugins";

        public string Key => "Lists2Playlists_SyncLists";

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromHours(6).Ticks
                }
            };
        }

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            try
            {
                progress?.Report(0);

                var config = _configurationService.GetConfiguration();
                if (!config.Enabled)
                {
                    progress?.Report(100);
                    return;
                }

                var totalLists = config.ListConfigurations.Count;
                var processedLists = 0;

                foreach (var listConfig in config.ListConfigurations)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        await _syncOrchestrator.SyncListAsync(listConfig, cancellationToken);
                        processedLists++;
                        progress?.Report((processedLists / (double)totalLists) * 100);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error syncing list: {ex.Message}");
                    }
                }

                progress?.Report(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in sync task: {ex.Message}");
                throw;
            }
        }
    }
}
