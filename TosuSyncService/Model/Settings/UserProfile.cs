using osuToolsV2.Rulesets.Legacy;

namespace TosuSyncService.Model.Settings;

public class UserProfile
{
    public TosuValue<UserLoginStatus> UserStatus { get; set; } = new TosuValue<UserLoginStatus>();
    public TosuValue<BanchoStatus> BanchoStatus { get; set; } = new TosuValue<BanchoStatus>();
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public TosuEnum<LegacyRuleset> Mode { get; set; } = new TosuEnum<LegacyRuleset>();
    public int RankedScore { get; set; }
    public double Level { get; set; }
    public double Accuracy { get; set; }
    public double Pp { get; set; }
    public int PlayCount { get; set; }
    public TosuEnum<CountryCodes> CountryCode { get; set; } = new TosuEnum<CountryCodes>();
    public string BackgroundColour { get; set; } = "";
}