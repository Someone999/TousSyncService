using System.Runtime.Versioning;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.EventSystem;
using TosuSyncService.IngameOverlay.Configs;
using TosuSyncService.Model;
using TosuSyncService.Model.Beatmaps;
using TosuSyncService.Model.Settings;

namespace TosuSyncService.IngameOverlay;

[SupportedOSPlatform("windows")]
public class IngameOverlayPlugin
{
    private readonly TosuEventManager _gosuEventManager;
    private string? _currentStatusString;
    private BreakTimeParser? _breakTimeParser;
    public ValueHolder<TosuData> Data { get; set; }
    public IngameOverlayPlugin(ValueHolder<TosuData> data, TosuEventManager gosuEventManager)
    {
        Data = data;
        _gosuEventManager = gosuEventManager;
    }
    public void Disable()
    {
        var tmp = Setting.OverlayConfigs.OverlayConfigItems.Select(c=>c).ToList();

        Setting.OverlayConfigs.OverlayConfigItems.Clear();
        Setting.OverlayConfigs.WriteToMmf(true);

        Setting.OverlayConfigs.OverlayConfigItems = tmp;
    }

    public void Enable()
    {
        var config = new OverlayConfig();
        var loader = new OverlayLoader();
        var gosuStatusChangedEvent =
            _gosuEventManager.GetEvent<TosuEvent<TosuGameState>>(TosuEventKeys.GameStateChanged);

        gosuStatusChangedEvent?.Bind((_, data) =>
        {
            _currentStatusString = data.EventValueChange.NewValue.ToString();

            foreach (var item in Setting.OverlayConfigs.OverlayConfigItems)
            {
                item.Visibility = item.VisibleStatus.Contains(_currentStatusString);
            }

            Setting.OverlayConfigs.WriteToMmf(false);
        });
        
        var gosuBeatmapChangedEvent =
            _gosuEventManager.GetEvent<TosuEvent<TosuBeatmap>>(TosuEventKeys.BeatmapChanged);

        gosuBeatmapChangedEvent?.Bind((_, data) =>
        {
            var beatmap = data.EventValueChange.NewValue;
            if (beatmap == null || Data.Value == null)
            {
                return;
            }
            
            _breakTimeParser = new BreakTimeParser(Data.Value);
        });

        var playerTimeChangedEvent =
            _gosuEventManager.GetEvent<TosuEvent<double>>(TosuEventKeys.PlayTimeChanged);

        playerTimeChangedEvent?.Bind((_, data) =>
        {
            if (_breakTimeParser == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_currentStatusString) || !_currentStatusString.StartsWith("Play"))
            {
                return;
            }
            
            bool updateMmf = false;

            foreach (var item in Setting.OverlayConfigs.OverlayConfigItems)
            {
                if(!item.VisibleStatus.Contains("Play"))continue;

                if (item.BreakTime == false)
                {
                    item.Visibility = true;
                    updateMmf = true;
                }
                else if (item is { BreakTime: true, Visibility: false })
                {
                    if (_breakTimeParser.InBreakTime((int)data.EventValueChange.NewValue))
                    {
                        item.Visibility = true;
                        updateMmf = true;
                    }
                }
                else if (item is { BreakTime: true, Visibility: true })
                {
                    if (!_breakTimeParser.InBreakTime((int)data.EventValueChange.NewValue))
                    {
                        item.Visibility = false;
                        updateMmf = true;
                    }
                }
            }

            if (updateMmf)
            {
                Setting.OverlayConfigs.WriteToMmf(false);
            }

        });
        
        foreach (var item in Setting.OverlayConfigs.OverlayConfigItems)
        {
            item.Visibility = false;
            item.VisibilityChanged += (list) =>
            {
                item.Visibility = item.VisibleStatus.Contains(_currentStatusString ?? "");
                Setting.OverlayConfigs.WriteToMmf(false);
            };
        }

        Setting.OverlayConfigs.WriteToMmf(true);
        
        //OverlayLoader.Inject();
    }
}