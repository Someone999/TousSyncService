using Newtonsoft.Json;
using osuToolsV2.Rulesets.Legacy;

namespace TosuSyncService.Model.Settings;

public class GameSettings
{
    public bool InterfaceVisible { get; set; }
    [JsonProperty("ReplayUIVisible")]
    public bool ReplayUiVisible { get; set; }
    [JsonProperty("chatVisibilityStatus")]
    public TosuEnum<ChatStatus> ChatStatus { get; set; } = new TosuEnum<ChatStatus>();
    public LeaderboardSettings Leaderboard { get; set; } = new LeaderboardSettings();
    public TosuEnum<ProgressBarType> ProgressBar { get; set; } = new TosuEnum<ProgressBarType>();
    public double BassDensity { get; set; }
    public Resolution Resolution { get; set; } = new Resolution();
    public ClientInfo Client { get; set; } = new ClientInfo();
    public ScoreMeterSettings ScoreMeter { get; set; } = new ScoreMeterSettings();
    public CursorSettings Cursor { get; set; } = new CursorSettings();
    public MouseSettings Mouse { get; set; } = new MouseSettings();
    public ManiaSettings Mania { get; set; } = new ManiaSettings();
    public TosuEnum<SortType> Sort { get; set; } = new TosuEnum<SortType>();
    public TosuEnum<GroupType> Group { get; set; } = new TosuEnum<GroupType>();
    public SkinSettings Skin { get; set; } = new SkinSettings();
    public TosuEnum<LegacyRuleset> Mode { get; set; } = new TosuEnum<LegacyRuleset>();
    public AudioSettings Audio { get; set; } = new AudioSettings();
    public BackgroundSettings Background { get; set; } = new BackgroundSettings();
    public KeyBindSettings KeyBinds { get; set; } = new KeyBindSettings();
}