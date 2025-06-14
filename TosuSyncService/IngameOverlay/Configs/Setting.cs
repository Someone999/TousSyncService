using System.Runtime.Versioning;

namespace TosuSyncService.IngameOverlay.Configs;

[SupportedOSPlatform("windows")]
public static class Setting
{
    public static string OsuExecPath = "";
    public static OverlayConfigs OverlayConfigs = new OverlayConfigs();
    public static GlobalConfig GlobalConfig = new GlobalConfig();
    public static bool AcceptEula = false;
}