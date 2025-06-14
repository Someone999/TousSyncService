namespace TosuSyncService.Model.Settings;

public class ClientInfo
{
    public bool UpdateAvailable { get; set; }
    public int Branch { get; set; }
    public string Version { get; set; } = "";
}