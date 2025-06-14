namespace TosuSyncService.Model.Settings;

public enum UserLoginStatus
{
    Reconnecting = 0,
    Guest = 256,
    ReceivingData = 257,
    Disconnected = 65537,
    Connected = 65793
}