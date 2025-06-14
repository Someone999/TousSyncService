namespace TosuSyncService.Model.Settings;

public class ScoreMeterSettings
{
    public TosuEnum<ScoreMeterType> ScoreMeterType { get; set; } = new TosuEnum<ScoreMeterType>();
    public int Size { get; set; }
}