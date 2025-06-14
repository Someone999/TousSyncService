namespace TosuSyncService.Mmf;

public interface IMmfNameNormalizer
{
    string Normalize(string mmfName, params object[] args);
}