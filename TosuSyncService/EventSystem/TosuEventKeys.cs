namespace TosuSyncService.EventSystem;

public static class TosuEventKeys
{
    public const string CountGekiChanged = "TosuEvent.CountGekiChanged->TosuEvent<int>";
    public const string CountKatuChanged = "TosuEvent.CountKatuChanged->TosuEvent<int>";
    public const string Count300Changed = "TosuEvent.Count300Changed->TosuEvent<int>";
    public const string Count100Changed = "TosuEvent.Count100Changed->TosuEvent<int>";
    public const string Count50Changed = "TosuEvent.Count50Changed->TosuEvent<int>";
    public const string CountMissChanged = "TosuEvent.CountMissChanged->TosuEvent<int>";
    public const string CountSliderBreakChanged = "TosuEvent.CountSliderBreakChanged->TosuEvent<int>";
    public const string BeatmapChanged = "TosuEvent.BeatmapChanged->TosuEvent<TosuBeatmap>";
    public const string AccuracyChanged = "TosuEvent.AccuracyChanged->TosuEvent<double>";
    public const string ComboChanged = "TosuEvent.ComboChanged->TosuEvent<int>";
    public const string HpChanged = "TosuEvent.HpChanged->TosuEvent<double>";
    public const string NameChanged = "TosuEvent.NameChanged->TosuEvent<string>";
    public const string LeaderboardPositionChanged = "TosuEvent.LeaderboardPositionChanged->TosuEvent<Leaderboard>";
    public const string ScoreChanged = "TosuEvent.ScoreChanged->TosuEvent<int>";
    public const string ModChanged = "TosuEvent.ModChanged->TosuEvent<int>";
    public const string GameModeChanged = "TosuEvent.GameModeChanged->TosuEvent<Ruleset>";
    public const string GradeChanged = "TosuEvent.GradeChanged->TosuEvent<GameGrade>";
    public const string TosuDataUpdated = "TosuEvent.TosuDataUpdated->TosuEvent<TosuData>";
    public const string GameStateChanged = "TosuEvent.GameStateChanged->TosuEvent<TosuMemoryState>";
    public const string PlayTimeChanged = "TosuEvent.PlayerTimeChanged->TosuEvent<double>";
    public const string Failed = "TosuEvent.Failed->TosuEvent<TosuData>";
    public const string NoFailHpZeroed = "TosuEvent.NoFailHpZeroed->TosuEvent<TosuData>";
    public const string Retry = "TosuEvent.Retry->TosuEvent<TosuData>";
}