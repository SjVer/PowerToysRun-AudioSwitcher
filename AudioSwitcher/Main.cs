using System.Windows.Controls;
using CoreAudioApi;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.AudioSwitcher
{
    static class Helpers
    {
        public static MMDevice[] GetDevices()
        {
            MMDeviceEnumerator DevEnum = new();

            MMDeviceCollection? DeviceCollection = null;
            try
            {
                DeviceCollection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eAll, EDeviceState.DEVICE_STATE_ACTIVE);
            }
            catch
            {
                // Error
                throw new System.Runtime.AmbiguousImplementationException(
                    "Error in parameter List - Failed to create the collection of all enabled MMDevice using MMDeviceEnumerator"
                );
            }

            List<MMDevice> devices = new();
            for (int i = 0; i < DeviceCollection.Count; i++)
                devices.Add(DeviceCollection[i]);

            return devices.ToArray();
        }    
    }

    public class Main : IPlugin, IPluginI18n, IContextMenu, ISettingProvider, IReloadable, IDisposable, IDelayedExecutionPlugin
    {
        private const string Setting = nameof(Setting);
        
        private string iconTheme = "light";

        // current value of the setting
        private bool _setting;

        private PluginInitContext _context;

        private string _iconPath;

        private bool _disposed;

        public string Name => Properties.Resources.plugin_name;

        public string Description => Properties.Resources.plugin_description;

        // TODO: remove dash from ID below and inside plugin.json
        public static string PluginID => "472a863a-949c-4bd5-b1d6-2cde31280d08";

        // TODO: add additional options (optional)
        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>()
        {
            new PluginAdditionalOption()
            {
                PluginOptionType= PluginAdditionalOption.AdditionalOptionType.Checkbox,
                Key = Setting,
                DisplayLabel = Properties.Resources.plugin_setting,
            },
        };

        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            _setting = settings?.AdditionalOptions?.FirstOrDefault(x => x.Key == Setting)?.Value ?? false;
        }

        // TODO: return context menus for each Result (optional)
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return new List<ContextMenuResult>(0);
        }

        // TODO: return query results
        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();
            foreach (var device in Helpers.GetDevices())
            {
                var useFor = device.DataFlow == EDataFlow.eRender ? ", used for audio playback"
                           : device.DataFlow == EDataFlow.eCapture ? ", used for audio recording"
                           : "";

                var deviceID = device.ID;

                results.Add(new Result
                {
                    IcoPath = $"Images\\Generated\\{device.DeviceDescription}.{iconTheme}.png",
                    Title = device.DeviceName,
                    SubTitle = device.FriendlyName + useFor,

                    Action = _ =>
                    {
                        PolicyConfigClient client = new();
                        client.SetDefaultEndpoint(deviceID, ERole.eMultimedia);
                        return true;
                    }
                });
            }

            return results;
        }

        // TODO: return delayed query results (optional)
        public List<Result> Query(Query query, bool delayedExecution)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();

            // empty query
            if (string.IsNullOrEmpty(query.Search))
            {
                return results;
            }

            return results;
        }

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        public string GetTranslatedPluginTitle()
        {
            return Properties.Resources.plugin_name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Properties.Resources.plugin_description;
        }

        private void OnThemeChanged(Theme oldTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                iconTheme = "light";
                _iconPath = "Images/AudioSwitcher.light.png";
            }
            else
            {
                iconTheme = "dark";
                _iconPath = "Images/AudioSwitcher.dark.png";
            }
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            if (_context is null)
            {
                return;
            }

            UpdateIconPath(_context.API.GetCurrentTheme());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_context != null && _context.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}
