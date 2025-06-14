namespace TosuSyncService.Model.Settings;

public class LeaderboardSettings
{
    public bool Visible { get; set; }
    public TosuEnum<LeaderboardType> LeaderboardType { get; set; } = new TosuEnum<LeaderboardType>();
}