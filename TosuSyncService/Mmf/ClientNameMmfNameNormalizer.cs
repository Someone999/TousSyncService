using TosuSyncService.Websockets;

namespace TosuSyncService.Mmf;

public class ClientNameMmfNameNormalizer : IMmfNameNormalizer
{
    public string Normalize(string mmfName, params object[] args)
    {
        if (args is not [TosuWebSocketClient client])
        {
            throw new ArgumentException(null, nameof(args));
        }

        return $"{client.ClientId}.{mmfName}";
    }
}