using HsManEventSystem.EventManagers;
using osuToolsV2.Rulesets.Legacy;
using TosuSyncService.Model;
using TosuSyncService.Model.Beatmaps;
using TosuSyncService.Model.Gameplays;
using TosuSyncService.Model.Settings;

namespace TosuSyncService.EventSystem;

public class TosuEventManager : EventManager
{
    public TosuEventManager()
    {
        RegisterEvent(TosuEventKeys.CountGekiChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.CountKatuChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.Count300Changed, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.Count100Changed, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.Count50Changed, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.CountMissChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.CountSliderBreakChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.BeatmapChanged, new TosuEvent<TosuBeatmap>());
        RegisterEvent(TosuEventKeys.AccuracyChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.ComboChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.HpChanged, new TosuEvent<double>());
        RegisterEvent(TosuEventKeys.NameChanged, new TosuEvent<string>());
        RegisterEvent(TosuEventKeys.LeaderboardPositionChanged, new TosuEvent<LeaderboardSlot>());
        RegisterEvent(TosuEventKeys.ScoreChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.ModChanged, new TosuEvent<int>());
        RegisterEvent(TosuEventKeys.GameModeChanged, new TosuEvent<LegacyRuleset>());
        RegisterEvent(TosuEventKeys.GradeChanged, new TosuEvent<string>());
        RegisterEvent(TosuEventKeys.TosuDataUpdated, new TosuEvent<TosuData>());
        RegisterEvent(TosuEventKeys.GameStateChanged, new TosuEvent<TosuGameState>());
        RegisterEvent(TosuEventKeys.PlayTimeChanged, new TosuEvent<double>());
        RegisterEvent(TosuEventKeys.Failed, new TosuEvent<TosuData>());
        RegisterEvent(TosuEventKeys.NoFailHpZeroed, new TosuEvent<TosuData>());
        RegisterEvent(TosuEventKeys.Retry, new TosuEvent<TosuData>());
    }
}